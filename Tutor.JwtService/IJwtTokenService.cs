using System.Security.Claims;

namespace Tutor.JwtService
{
    public interface IJwtTokenService
    {
        string GenerateToken(string email, Dictionary<string, string> claims, int expiryMinutes = 1440);
        TokenValidationResult ValidateToken(string token);
    }
}
