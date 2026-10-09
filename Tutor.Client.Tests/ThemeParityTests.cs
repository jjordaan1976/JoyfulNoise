using System.Text.RegularExpressions;
using MudBlazor.Utilities;
using Tutor.Client.Theme;

namespace Tutor.Client.Tests;

/// <summary>
/// The palette exists twice — C# for MudBlazor, CSS custom properties for everything else.
/// These tests stop the two from drifting apart.
/// </summary>
public class ThemeParityTests
{
    private static Dictionary<string, string> CssTokens()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "theme.css"));
        return Regex.Matches(css, @"--([a-z0-9-]+)\s*:\s*(#[0-9a-fA-F]{6})\s*;")
                    .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value.ToLowerInvariant());
    }

    [Theory]
    [InlineData("c-bg", BrandColors.Background)]
    [InlineData("c-bg-card", BrandColors.Surface)]
    [InlineData("c-bg-raised", BrandColors.Raised)]
    [InlineData("c-surface", BrandColors.Surface)]
    [InlineData("c-text", BrandColors.Text)]
    [InlineData("c-text-muted", BrandColors.TextMuted)]
    [InlineData("c-text-dim", BrandColors.TextDim)]
    [InlineData("c-border", BrandColors.Border)]
    [InlineData("c-accent", BrandColors.Pink)]
    [InlineData("c-accent-deep", BrandColors.AccentDeep)]
    [InlineData("c-on-accent", BrandColors.OnAccent)]
    [InlineData("c-brand-mint", BrandColors.Mint)]
    [InlineData("c-brand-sage", BrandColors.Sage)]
    [InlineData("c-brand-amber", BrandColors.Amber)]
    [InlineData("c-brand-coral", BrandColors.Coral)]
    [InlineData("c-brand-pink", BrandColors.Pink)]
    [InlineData("c-brand-orchid", BrandColors.Orchid)]
    [InlineData("c-success", BrandColors.Success)]
    [InlineData("c-warn", BrandColors.Warn)]
    [InlineData("c-danger", BrandColors.Danger)]
    public void CssToken_MatchesBrandColour(string token, string expected)
    {
        var tokens = CssTokens();

        Assert.True(tokens.ContainsKey(token), $"theme.css does not define --{token}");
        Assert.Equal(expected.ToLowerInvariant(), tokens[token]);
    }

    private static string Hex(MudColor color) => color.ToString(MudColorOutputFormats.Hex).ToLowerInvariant();

    [Fact]
    public void MudPalette_UsesTheBrandColours()
    {
        var p = TutorTheme.Light.PaletteLight;

        Assert.Equal(BrandColors.Pink, Hex(p.Primary));
        Assert.Equal(BrandColors.OnAccent, Hex(p.PrimaryContrastText));
        Assert.Equal(BrandColors.TextMuted, Hex(p.Secondary));
        Assert.Equal(BrandColors.White, Hex(p.SecondaryContrastText));
        Assert.Equal(BrandColors.Background, Hex(p.Background));
        Assert.Equal(BrandColors.Surface, Hex(p.Surface));
        Assert.Equal(BrandColors.Surface, Hex(p.AppbarBackground));
        Assert.Equal(BrandColors.Text, Hex(p.AppbarText));
        Assert.Equal(BrandColors.Text, Hex(p.TextPrimary));
        Assert.Equal(BrandColors.TextMuted, Hex(p.TextSecondary));
        Assert.Equal(BrandColors.Border, Hex(p.Divider));
        Assert.Equal(BrandColors.Border, Hex(p.TableLines));
    }

    [Fact]
    public void MudPalette_StatusColours_AreTheAccessibleOnes()
    {
        var p = TutorTheme.Light.PaletteLight;

        Assert.Equal(BrandColors.Success, Hex(p.Success));
        Assert.Equal(BrandColors.Warn, Hex(p.Warning));
        Assert.Equal(BrandColors.Danger, Hex(p.Error));
    }

    [Fact]
    public void Typography_UsesInterFirst()
    {
        var t = TutorTheme.Light.Typography;

        Assert.Equal("Inter", t.Default.FontFamily[0]);
        Assert.Equal("Inter", t.H5.FontFamily[0]);
        Assert.Equal("Inter", t.H6.FontFamily[0]);
        Assert.Equal("Inter", t.Subtitle1.FontFamily[0]);
        Assert.Equal("Inter", t.Body2.FontFamily[0]);
        Assert.Equal("Inter", t.Button.FontFamily[0]);
        Assert.Equal("Inter", t.Caption.FontFamily[0]);
    }
}
