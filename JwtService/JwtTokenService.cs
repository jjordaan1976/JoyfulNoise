using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtService;

public class JwtTokenService : IJwtTokenService
{
    private const string UserIdClaimType = "uid";

    public string CreateToken(string userId, string secret, DateTime expiration)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId must not be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("secret must not be empty.", nameof(secret));

        var key = BuildKey(secret);
        var utcExpiry = expiration.ToUniversalTime();
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(UserIdClaimType, userId)]),
            NotBefore = utcExpiry.AddHours(-24),
            Expires = utcExpiry,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    public string ValidateToken(string token, string secret)
    {
        var principal = GetPrincipal(token, secret, validateLifetime: true);
        return ExtractUserId(principal);
    }

    public string RotateToken(string token, string secret, DateTime newExpiration)
    {
        var principal = GetPrincipal(token, secret, validateLifetime: true);
        var userId = ExtractUserId(principal);
        return CreateToken(userId, secret, newExpiration);
    }

    private static ClaimsPrincipal GetPrincipal(string token, string secret, bool validateLifetime)
    {
        var key = BuildKey(secret);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ValidateToken(token, parameters, out _);
        }
        catch (SecurityTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token is invalid.", ex);
        }
    }

    private static string ExtractUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(UserIdClaimType)
            ?? throw new SecurityTokenException($"Token is missing the '{UserIdClaimType}' claim.");
        return claim.Value;
    }

    private static SymmetricSecurityKey BuildKey(string secret) =>
        new(Encoding.UTF8.GetBytes(secret));
}
