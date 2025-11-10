using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Nestelia.Domain.Common.ViewModels.Auth;
using Nestelia.Domain.DTO.Auth;
using Nestelia.Domain.Entities;
using Nestelia.Domain.Entities.Auth;
using Nestelia.Infraestructure.Common;
using Nestelia.Infraestructure.Interfaces.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nestelia.Infraestructure.Repositories.Auth
{
    public class TokenRepository(ApplicationDbContext context, IConfiguration config) : ITokenRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IConfiguration _config = config;

        public async Task<TokenResponse> GenerateTokens(ApplicationUser user, UserSession userSession)
        {
            var accessToken = GenerateAccessToken(userSession);

            var refreshToken = await GenerateRefreshToken(user);

            if (refreshToken == "Error al generar el refresh token")
                throw new Exception("No se pudo generar el refresh token.");

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateAccessToken(UserSession user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(30), // PA PRUEBA, ANTES MINUTOPS
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var newAccessToken = new RefreshToken
            {
                Active = true,
                Expiration = DateTime.UtcNow.AddMinutes(60 * 24 * 7),
                RefreshTokenValue = Guid.NewGuid().ToString("N"),
                Used = false,
                UserId = user.Id
            };

            await _context.RefreshTokens.AddAsync(newAccessToken);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected == 0) 
                return "Error al generar el refresh token";

            return newAccessToken.RefreshTokenValue;
        }

    }
}
