using System.Security.Claims;

namespace Tutor.JwtService
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public ClaimsPrincipal? Principal { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
