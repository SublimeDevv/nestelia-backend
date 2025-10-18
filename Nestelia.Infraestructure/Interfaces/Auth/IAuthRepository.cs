using Nestelia.Domain.Common.ViewModels.Auth;

namespace Nestelia.Infraestructure.Interfaces.Auth
{
    public interface IAuthRepository
    {
        Task<TokenResponse> RefreshToken(string request);
        Task<UserVM> GetUserById(string id);
    }
}
