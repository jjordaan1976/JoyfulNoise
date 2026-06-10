using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Tutor.JwtService;

namespace Tutor.JwtService.Tests
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _service;
        private readonly string _secretKey = "very-secret-key-that-is-long-enough-for-hmacsha256-algorithm";
        private readonly string _issuer = "TutorApp";
        private readonly string _audience = "TutorUsers";

        public JwtTokenServiceTests()
        {
            _service = new JwtTokenService(_secretKey, _issuer, _audience);
        }

        [Fact]
        public void GenerateToken_WithValidClaims_ReturnsValidToken()
        {
            var email = "test@example.com";
            var userId = "123";
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Email, email },
                { "userId", userId }
            };

            var token = _service.GenerateToken(email, claims);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ReturnsSuccess()
        {
            var email = "test@example.com";
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Email, email }
            };
            var token = _service.GenerateToken(email, claims);

            var result = _service.ValidateToken(token);

            Assert.True(result.IsValid);
            Assert.NotNull(result.Principal);
        }

        [Fact]
        public void ValidateToken_WithValidToken_ExtractsEmailClaim()
        {
            var email = "test@example.com";
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Email, email }
            };
            var token = _service.GenerateToken(email, claims);

            var result = _service.ValidateToken(token);

            var emailClaim = result.Principal?.FindFirst(ClaimTypes.Email);
            Assert.NotNull(emailClaim);
            Assert.Equal(email, emailClaim.Value);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ReturnsFalse()
        {
            var invalidToken = "invalid.token.here";

            var result = _service.ValidateToken(invalidToken);

            Assert.False(result.IsValid);
            Assert.Null(result.Principal);
        }

        [Fact]
        public void ValidateToken_WithTamperedToken_ReturnsFalse()
        {
            var email = "test@example.com";
            var claims = new Dictionary<string, string>
            {
                { ClaimTypes.Email, email }
            };
            var token = _service.GenerateToken(email, claims);

            // Tamper with the token
            var parts = token.Split('.');
            var tamperedToken = parts[0] + ".tampered." + parts[2];

            var result = _service.ValidateToken(tamperedToken);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void GenerateToken_CreatesTokenWithExpiration()
        {
            var email = "test@example.com";
            var claims = new Dictionary<string, string>();
            var expiryMinutes = 60;

            var token = _service.GenerateToken(email, claims, expiryMinutes);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            Assert.NotNull(jwtToken);
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }
    }
}
