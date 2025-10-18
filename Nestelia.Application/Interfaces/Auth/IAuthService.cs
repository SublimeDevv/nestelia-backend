using Microsoft.AspNetCore.Http;
using Nestelia.Domain.Common.ViewModels.Auth;
using Nestelia.Domain.DTO.Auth;
using Nestelia.Domain.Entities;
using Nestelia.Domain.Shared;

namespace Nestelia.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<Result<ApplicationUser>> CreateAccount(UserDto UserDto);
        Task<Result<ApplicationUser>> ValidateUser(LoginDto LoginDto);
        Task<Result<UserVM>> GetUserById(string id);
        Task<TokenResponse> CreateTokens(ApplicationUser user);
        Task<Result<TokenResponse>> RefreshToken(string request);
        void SetTokensInsideCookie(TokenResponse tokenDto, HttpContext context);
    }
}
