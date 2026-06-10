using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtService;

public class ActivationTokenService : IActivationTokenService
{
    private const string EmailClaimType     = "act_email";
    private const string CompanyIdClaimType = "act_company";

    public string CreateToken(string email, int companyId, string secret)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("email must not be empty.", nameof(email));
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("secret must not be empty.", nameof(secret));

        var key        = BuildKey(secret);
        var expiry     = DateTime.UtcNow.AddDays(3);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(EmailClaimType,     email),
                new Claim(CompanyIdClaimType, companyId.ToString())
            ]),
            Expires            = expiry,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }

    public (string Email, int CompanyId) ValidateToken(string token, string secret)
    {
        var key        = BuildKey(secret);
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer           = false,
            ValidateAudience         = false,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = key,
            ClockSkew                = TimeSpan.Zero
        };

        var handler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal;

        try
        {
            principal = handler.ValidateToken(token, parameters, out _);
        }
        catch (SecurityTokenException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Activation token is invalid.", ex);
        }

        var email = principal.FindFirst(EmailClaimType)?.Value
            ?? throw new SecurityTokenException($"Token is missing the '{EmailClaimType}' claim.");

        var companyIdRaw = principal.FindFirst(CompanyIdClaimType)?.Value
            ?? throw new SecurityTokenException($"Token is missing the '{CompanyIdClaimType}' claim.");

        if (!int.TryParse(companyIdRaw, out var companyId))
            throw new SecurityTokenException("Token contains an invalid company ID.");

        return (email, companyId);
    }

    private static SymmetricSecurityKey BuildKey(string secret) =>
        new(Encoding.UTF8.GetBytes(secret));
}
