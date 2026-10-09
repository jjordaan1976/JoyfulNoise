using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Tutor.Web.Services;

namespace Tutor.Client.Tests;

public class WebAuthenticationStateProviderTests
{
    private readonly Mock<IAuthService> _authMock = new();

    private JwtAuthenticationStateProvider CreateSut(Dictionary<string, string?>? settings = null) =>
        new(_authMock.Object,
            new ConfigurationBuilder().AddInMemoryCollection(settings ?? new()).Build());

    private static string TokenWithPayload(string json) =>
        "header." + Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                           .TrimEnd('=').Replace('+', '-').Replace('/', '_') + ".signature";

    // ── Authentication switched off (bypass) ──────────────────────────────────

    [Fact]
    public async Task WhenAuthDisabled_ReturnsAuthenticatedBypassUser()
    {
        var sut = CreateSut(new() { ["Auth:Enabled"] = "false", ["Auth:BypassEmail"] = "adele@x.com" });

        var state = await sut.GetAuthenticationStateAsync();

        Assert.True(state.User.Identity!.IsAuthenticated);
        Assert.Equal("adele@x.com", state.User.FindFirst(ClaimTypes.Email)?.Value);
    }

    [Fact]
    public async Task WhenAuthDisabled_DoesNotConsultStoredToken()
    {
        var sut = CreateSut(new() { ["Auth:Enabled"] = "false", ["Auth:BypassEmail"] = "adele@x.com" });

        await sut.GetAuthenticationStateAsync();

        _authMock.Verify(a => a.GetTokenAsync(), Times.Never);
    }

    [Fact]
    public async Task WhenAuthDisabled_WithoutBypassEmail_ReturnsAnonymous()
    {
        var sut = CreateSut(new() { ["Auth:Enabled"] = "false" });

        var state = await sut.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
    }

    // ── Authentication on (the default) ───────────────────────────────────────

    [Fact]
    public async Task WhenSettingMissing_AuthIsEnabled_AndNoTokenIsAnonymous()
    {
        _authMock.Setup(a => a.GetTokenAsync()).ReturnsAsync((string?)null);
        var sut = CreateSut();

        var state = await sut.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
    }

    [Fact]
    public async Task WhenAuthEnabled_WithValidToken_ReturnsClaimsFromToken()
    {
        _authMock.Setup(a => a.GetTokenAsync())
                 .ReturnsAsync(TokenWithPayload("{\"name\":\"Amy Bell\",\"studentId\":\"5\"}"));
        var sut = CreateSut(new() { ["Auth:Enabled"] = "true" });

        var state = await sut.GetAuthenticationStateAsync();

        Assert.True(state.User.Identity!.IsAuthenticated);
        Assert.Equal("Amy Bell", state.User.FindFirst("name")?.Value);
        Assert.Equal("5", state.User.FindFirst("studentId")?.Value);
    }

    [Fact]
    public async Task WhenAuthEnabled_WithMalformedToken_ReturnsAnonymous()
    {
        _authMock.Setup(a => a.GetTokenAsync()).ReturnsAsync("not-a-jwt");
        var sut = CreateSut(new() { ["Auth:Enabled"] = "true" });

        var state = await sut.GetAuthenticationStateAsync();

        Assert.False(state.User.Identity!.IsAuthenticated);
    }
}
