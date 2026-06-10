namespace JwtService;

public interface IActivationTokenService
{
    string CreateToken(string email, int companyId, string secret);
    (string Email, int CompanyId) ValidateToken(string token, string secret);
}
