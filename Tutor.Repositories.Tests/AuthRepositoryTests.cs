using Moq;
using Tutor.Data.Implementations;
using Tutor.Data.Interfaces;
using Tutor.Data.Models;
using Tutor.JwtService;
using Tutor.Repositories;

namespace Tutor.Repositories.Tests;

public class AuthRepositoryTests
{
    private readonly Mock<IEmailService> _emailMock;
    private readonly Mock<IOtpService> _otpMock;
    private readonly Mock<IJwtTokenService> _jwtMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly AuthRepository _sut;

    public AuthRepositoryTests()
    {
        _emailMock    = new Mock<IEmailService>();
        _otpMock      = new Mock<IOtpService>();
        _jwtMock      = new Mock<IJwtTokenService>();
        _userRepoMock = new Mock<IUserRepository>();
        _sut          = new AuthRepository(_emailMock.Object, _otpMock.Object, _jwtMock.Object, _userRepoMock.Object);
    }

    // ── RequestOtpAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task RequestOtpAsync_WithEmptyEmail_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RequestOtpAsync(""));
    }

    [Fact]
    public async Task RequestOtpAsync_WhenSuccessful_StoresOtpAndSendsEmail()
    {
        _otpMock.Setup(o => o.GenerateOtp()).Returns("123456");
        _emailMock.Setup(e => e.SendAsync("a@b.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(true);

        var result = await _sut.RequestOtpAsync("a@b.com");

        Assert.True(result);
        _otpMock.Verify(o => o.StoreOtp("a@b.com", "123456", It.IsAny<int>()), Times.Once);
        _emailMock.Verify(e => e.SendAsync("a@b.com", It.IsAny<string>(), It.IsAny<string>(),
            It.Is<string>(body => body.Contains("123456"))), Times.Once);
    }

    [Fact]
    public async Task RequestOtpAsync_WhenEmailSendFails_Throws()
    {
        _otpMock.Setup(o => o.GenerateOtp()).Returns("123456");
        _emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RequestOtpAsync("a@b.com"));
    }

    // ── VerifyOtpAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task VerifyOtpAsync_WithEmptyInputs_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.VerifyOtpAsync("", "123456"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.VerifyOtpAsync("a@b.com", ""));
    }

    [Fact]
    public async Task VerifyOtpAsync_WithInvalidOtp_Throws()
    {
        _otpMock.Setup(o => o.ValidateOtp("a@b.com", "000000")).Returns(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.VerifyOtpAsync("a@b.com", "000000"));
    }

    [Fact]
    public async Task VerifyOtpAsync_WhenNoUserRegistered_Throws()
    {
        _otpMock.Setup(o => o.ValidateOtp("a@b.com", "123456")).Returns(true);
        _userRepoMock.Setup(u => u.GetByEmailAsync("a@b.com")).ReturnsAsync(Enumerable.Empty<User>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.VerifyOtpAsync("a@b.com", "123456"));
    }

    [Fact]
    public async Task VerifyOtpAsync_WhenValid_ReturnsTokenWithIdentityClaims()
    {
        _otpMock.Setup(o => o.ValidateOtp("a@b.com", "123456")).Returns(true);
        _userRepoMock.Setup(u => u.GetByEmailAsync("a@b.com")).ReturnsAsync(new List<User>
        {
            new() { Email = "a@b.com", DisplayName = "Amy Bell", Role = UserRole.Student, StudentID = 5 },
            new() { Email = "a@b.com", DisplayName = "Amy Bell", Role = UserRole.AccountHolder, AccountHolderID = 9 }
        });

        Dictionary<string, string>? capturedClaims = null;
        _jwtMock.Setup(j => j.GenerateToken("a@b.com", It.IsAny<Dictionary<string, string>>(), It.IsAny<int>()))
                .Callback<string, Dictionary<string, string>, int>((_, claims, _) => capturedClaims = claims)
                .Returns("the-token");

        var token = await _sut.VerifyOtpAsync("a@b.com", "123456");

        Assert.Equal("the-token", token);
        Assert.NotNull(capturedClaims);
        Assert.Equal("Amy Bell", capturedClaims["name"]);
        Assert.Equal("Student,AccountHolder", capturedClaims["role"]);
        Assert.Equal("5", capturedClaims["studentId"]);
        Assert.Equal("9", capturedClaims["accountHolderId"]);
        Assert.False(capturedClaims.ContainsKey("teacherId"));
    }
}
