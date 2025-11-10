using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Nestelia.Application.Interfaces.Auth;
using Nestelia.Domain.Common.ViewModels.Auth;
using Nestelia.Domain.DTO.Auth;
using Nestelia.Domain.Entities;
using Nestelia.Domain.Shared;
using Nestelia.Infraestructure.Interfaces.Auth;

namespace Nestelia.Application.Services.Auth
{
    public class AuthService(IHttpContextAccessor httpContextAccessor, IAuthRepository authRepository, ITokenRepository tokenRepository, RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager) :  IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly ITokenRepository _tokenRepository = tokenRepository;
        private readonly HttpContext context = httpContextAccessor.HttpContext!;
        
        public async Task<Result<ApplicationUser>> CreateAccount(UserDto UserDto)
        {

            if (UserDto is null) return Result.Failure<ApplicationUser>("Los datos del usuario no pueden ser nulos.");

            var newUser = new ApplicationUser()
            {
                Email = UserDto.Email,
                PasswordHash = UserDto.Password,
                UserName = UserDto.Email
            };

            var user = await _userManager.FindByEmailAsync(newUser.Email);
            if (user is not null) return Result.Failure<ApplicationUser>("El correo electrónico ya está en uso.");

            var createUser = await _userManager.CreateAsync(newUser, UserDto.Password);
            if (!createUser.Succeeded) return Result.Failure<ApplicationUser>("Error al crear la cuenta.");

            var checkAdmin = await _roleManager.FindByNameAsync("Admin");
            if (checkAdmin is null)
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
                await _userManager.AddToRoleAsync(newUser, "Admin");
                return Result.Success(newUser);
            }
            else
            {
                var checkUser = await _roleManager.FindByNameAsync("User");
                if (checkUser is null) await _roleManager.CreateAsync(new IdentityRole() { Name = "User" });

                await _userManager.AddToRoleAsync(newUser, "User");

                return Result.Success(newUser);
            }
        }

        public async Task<Result<ApplicationUser>> ValidateUser(LoginDto LoginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(LoginDto.Email);
                if (user is null) return Result.Failure<ApplicationUser>("Credenciales inválidas.");

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, LoginDto.Password);
                if (!isPasswordValid) return Result.Failure<ApplicationUser>("Credenciales inválidas.");

                return Result.Success(user);
            }
            catch (Exception)
            {
                return Result.Failure<ApplicationUser>("Error al validar usuario.");
            }
        }

        public async Task<Result<UserVM>> GetUserById(string id)
        {
            try
            {
                var user = await _authRepository.GetUserById(id);
                if (user is null) return Result.Failure<UserVM>("Usuario no encontrado.");
                return Result.Success(user);
            }
            catch (Exception)
            {
                return Result.Failure<UserVM>("Error al obtener usuario.");
            }
        }

        public async Task<TokenResponse> CreateTokens(ApplicationUser user)
        {

            var userRoles = await _userManager.GetRolesAsync(user);

            var userSession = new UserSession(
                Id: user.Id,
                Email: user.Email ?? string.Empty,
                Role: userRoles.FirstOrDefault() ?? "User"
            );

            TokenResponse tokenResponse = await _tokenRepository.GenerateTokens(user, userSession);

            return tokenResponse;
        }

        public async Task<Result<TokenResponse>> RefreshToken(string request)
        {
            try
            {
                var refreshToken = await _authRepository.RefreshToken(request);
                return Result.Success(refreshToken, "Tokens refrescados correctamente");
            }
            catch (Exception e)
            {
                return Result.Failure<TokenResponse>($"Error al refrescar token: {e.Message}");
            }
        }

        public string GetIdentity()
        {
            var claim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim)) return string.Empty;
            return claim;
        }

        public void SetTokensInsideCookie(TokenResponse tokenResponse)
        {

            CookieOptions cookieOptions = GetCookiesOptions();

            context.Response.Cookies.Append("accessToken", tokenResponse.AccessToken ?? string.Empty,
                new CookieOptions
                {
                    HttpOnly = cookieOptions.HttpOnly,
                    Expires = DateTime.UtcNow.AddDays(5), // pa pruiebas, cambiar luego a minutos
                    SameSite = cookieOptions.SameSite,
                    Secure = cookieOptions.Secure,
                    IsEssential = cookieOptions.IsEssential
                });

            context.Response.Cookies.Append("refreshToken", tokenResponse.RefreshToken ?? string.Empty,
                new CookieOptions
                {
                    HttpOnly = cookieOptions.HttpOnly,
                    Expires = DateTime.UtcNow.AddDays(7),
                    SameSite = cookieOptions.SameSite,
                    Secure = cookieOptions.Secure,
                    IsEssential = cookieOptions.IsEssential
                });
        }

        public void RemoveTokensFromCookies()
        {
            var cookieOptions = GetCookiesOptions();

            context.Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = cookieOptions.HttpOnly,
                SameSite = cookieOptions.SameSite,
                Secure = cookieOptions.Secure,
                IsEssential = cookieOptions.IsEssential,
                Path = cookieOptions.Path,
                Domain = cookieOptions.Domain
            });

            context.Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = cookieOptions.HttpOnly,
                SameSite = cookieOptions.SameSite,
                Secure = cookieOptions.Secure,
                IsEssential = cookieOptions.IsEssential,
                Path = cookieOptions.Path,
                Domain = cookieOptions.Domain
            });
        }

        private CookieOptions GetCookiesOptions()
        {
            var isProduction = context.Request.IsHttps;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isProduction,
                SameSite = isProduction ? SameSiteMode.None : SameSiteMode.Lax,
                IsEssential = true,
            };

            return cookieOptions;

        }

    }
}
