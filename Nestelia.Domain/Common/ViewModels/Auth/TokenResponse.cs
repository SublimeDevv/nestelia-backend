namespace Nestelia.Domain.Common.ViewModels.Auth
{
    public class TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

}
