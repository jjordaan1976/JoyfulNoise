using Microsoft.IdentityModel.Tokens;

namespace JwtService.Tests;

public class JwtTokenServiceTests
{
    private const string Secret = "super-secret-key-that-is-long-enough-for-hmac-sha256";
    private const string UserId = "user-42";

    private readonly JwtTokenService tokenService = new();

    // --- CreateToken ---

    [Fact]
    public void CreateToken_ReturnsNonEmptyString()
    {
        var token = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        Assert.NotEmpty(token);
    }

    [Fact]
    public void CreateToken_ThrowsArgumentException_WhenUserIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            tokenService.CreateToken(string.Empty, Secret, DateTime.UtcNow.AddHours(1)));
    }

    [Fact]
    public void CreateToken_ThrowsArgumentException_WhenSecretIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            tokenService.CreateToken(UserId, string.Empty, DateTime.UtcNow.AddHours(1)));
    }

    // --- ValidateToken ---

    [Fact]
    public void ValidateToken_ReturnsUserId_ForValidToken()
    {
        var token = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        var result = tokenService.ValidateToken(token, Secret);
        Assert.Equal(UserId, result);
    }

    [Fact]
    public void ValidateToken_ThrowsSecurityTokenExpiredException_ForExpiredToken()
    {
        var token = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddSeconds(-1));
        Assert.Throws<SecurityTokenExpiredException>(() =>
            tokenService.ValidateToken(token, Secret));
    }

    [Fact]
    public void ValidateToken_ThrowsSecurityTokenInvalidSignatureException_ForTamperedToken()
    {
        var token = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        var tampered = token[..^5] + "XXXXX";
        Assert.Throws<SecurityTokenInvalidSignatureException>(() =>
            tokenService.ValidateToken(tampered, Secret));
    }

    [Fact]
    public void ValidateToken_ThrowsSecurityTokenException_ForGarbage()
    {
        Assert.ThrowsAny<SecurityTokenException>(() =>
            tokenService.ValidateToken("not.a.jwt", Secret));
    }

    // --- RotateToken ---

    [Fact]
    public void RotateToken_ReturnsNewToken_WithNewExpiry()
    {
        var original = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        var newExpiry = DateTime.UtcNow.AddHours(2);
        var rotated = tokenService.RotateToken(original, Secret, newExpiry);
        Assert.NotEmpty(rotated);
        Assert.NotEqual(original, rotated);
    }

    [Fact]
    public void RotateToken_PreservesUserId()
    {
        var original = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        var rotated = tokenService.RotateToken(original, Secret, DateTime.UtcNow.AddHours(2));
        var userId = tokenService.ValidateToken(rotated, Secret);
        Assert.Equal(UserId, userId);
    }

    [Fact]
    public void RotateToken_ThrowsSecurityTokenExpiredException_ForExpiredToken()
    {
        var expired = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddSeconds(-1));
        Assert.Throws<SecurityTokenExpiredException>(() =>
            tokenService.RotateToken(expired, Secret, DateTime.UtcNow.AddHours(1)));
    }

    [Fact]
    public void RotateToken_ThrowsSecurityTokenInvalidSignatureException_ForInvalidToken()
    {
        var token = tokenService.CreateToken(UserId, Secret, DateTime.UtcNow.AddHours(1));
        var tampered = token[..^5] + "XXXXX";
        Assert.Throws<SecurityTokenInvalidSignatureException>(() =>
            tokenService.RotateToken(tampered, Secret, DateTime.UtcNow.AddHours(1)));
    }
}
