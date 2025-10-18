using Nestelia.Domain.Common.ViewModels.Auth;
using Nestelia.Domain.DTO.Auth;
using Nestelia.Domain.Entities;

namespace Nestelia.Infraestructure.Interfaces.Auth
{
    public interface ITokenRepository
    {
        Task<TokenResponse> GenerateTokens(ApplicationUser user, UserSession userSession);
    }
}
