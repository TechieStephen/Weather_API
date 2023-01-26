using weatherapi.Entities;

namespace weatherapi.Services.Authentication
{
    public interface IJwtAuthentication
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
    }
}
