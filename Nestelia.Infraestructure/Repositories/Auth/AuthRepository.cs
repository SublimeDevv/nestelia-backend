using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nestelia.Domain.Common.ViewModels.Auth;
using Nestelia.Domain.DTO.Auth;
using Nestelia.Domain.Entities;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Auth;

namespace Nestelia.Infraestructure.Repositories.Auth
{
    class AuthRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        ITokenRepository tokenRepository, ApplicationDbContext context) : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly ApplicationDbContext _context = context;

        public async Task<TokenResponse> RefreshToken(string request)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(q => q.RefreshTokenValue == request);

            if (refreshToken is null ||
                refreshToken.Active == false ||
                refreshToken.Expiration <= DateTime.UtcNow)
            {
                throw new ForbiddenAccessException();
            }

            if (refreshToken.Used)
            {

                var refreshTokens = await _context.RefreshTokens.Where(q => q.Active && q.Used == false && q.UserId == refreshToken.UserId)
                .ToListAsync();

                foreach (var rt in refreshTokens)
                {
                    rt.Used = true;
                    rt.Active = false;
                }

                await _context.SaveChangesAsync();

                throw new ForbiddenAccessException();
            }

            refreshToken.Used = true;

            var user = await _context.Users.FindAsync(refreshToken.UserId) ?? throw new ForbiddenAccessException();
            var getUserRole = await _userManager.GetRolesAsync(user);

            UserSession userSession = new(user.Id, user.Email, getUserRole.First());

            var generateTokens = await _tokenRepository.GenerateTokens(user, userSession);

            return generateTokens;

        }

        public async Task<UserVM> GetUserById(string id)
        {
            string query = @"
                    SELECT 
                        u.""Id"", 
                        u.""Email"", 
                        u.""UserName"",
                        r.""Name"" AS ""Role""
                    FROM ""AspNetUsers"" u
                    LEFT JOIN ""AspNetUserRoles"" ur ON u.""Id"" = ur.""UserId""
                    LEFT JOIN ""AspNetRoles"" r ON ur.""RoleId"" = r.""Id""
                    WHERE u.""Id"" = @id
                    ORDER BY ur.""RoleId"" DESC
                    LIMIT 1";

            var connection =  _context.Database.GetDbConnection();

            return await connection.QueryFirstAsync<UserVM>(query, new { id });
        }
    }
}
