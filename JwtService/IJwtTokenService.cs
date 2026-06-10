namespace JwtService;

public interface IJwtTokenService
{
    string CreateToken(string userId, string secret, DateTime expiration);
    string ValidateToken(string token, string secret);
    string RotateToken(string token, string secret, DateTime newExpiration);
}
