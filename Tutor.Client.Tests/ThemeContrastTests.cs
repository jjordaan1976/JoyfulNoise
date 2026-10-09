using Tutor.Client.Theme;

namespace Tutor.Client.Tests;

/// <summary>
/// Executable form of the colour rules in UI_SKILL.md: every text/fill pairing the theme
/// asks for must meet WCAG AA (4.5:1) on every surface it can sit on.
/// </summary>
public class ThemeContrastTests
{
    private const double AA = 4.5;

    private static double Channel(int value)
    {
        var s = value / 255.0;
        return s <= 0.03928 ? s / 12.92 : Math.Pow((s + 0.055) / 1.055, 2.4);
    }

    private static double Luminance(string hex) =>
        0.2126 * Channel(Convert.ToInt32(hex.Substring(1, 2), 16)) +
        0.7152 * Channel(Convert.ToInt32(hex.Substring(3, 2), 16)) +
        0.0722 * Channel(Convert.ToInt32(hex.Substring(5, 2), 16));

    internal static double Contrast(string a, string b)
    {
        var la = Luminance(a);
        var lb = Luminance(b);
        return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05);
    }

    /// <summary>The surfaces text can sit on: cards, the page, and grey inner panels.</summary>
    public static TheoryData<string> Surfaces => new()
    {
        BrandColors.Surface, BrandColors.Background, BrandColors.Raised
    };

    public static TheoryData<string> BrandFills => new()
    {
        BrandColors.Mint, BrandColors.Sage, BrandColors.Amber,
        BrandColors.Coral, BrandColors.Pink, BrandColors.Orchid
    };

    [Theory, MemberData(nameof(Surfaces))]
    public void BodyText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.Text, surface) >= AA);

    [Theory, MemberData(nameof(Surfaces))]
    public void MutedText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.TextMuted, surface) >= AA);

    [Theory, MemberData(nameof(Surfaces))]
    public void DeepPinkText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.AccentDeep, surface) >= AA);

    [Theory, MemberData(nameof(Surfaces))]
    public void SuccessText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.Success, surface) >= AA);

    [Theory, MemberData(nameof(Surfaces))]
    public void WarningText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.Warn, surface) >= AA);

    [Theory, MemberData(nameof(Surfaces))]
    public void DangerText_IsReadable_OnEverySurface(string surface) =>
        Assert.True(Contrast(BrandColors.Danger, surface) >= AA);

    [Theory, MemberData(nameof(BrandFills))]
    public void BlackText_IsReadable_OnEveryBrandFill(string fill) =>
        Assert.True(Contrast(BrandColors.Text, fill) >= AA);

    [Fact]
    public void PrimaryButtonLabel_IsReadable_OnPink() =>
        Assert.True(Contrast(BrandColors.OnAccent, BrandColors.Pink) >= AA);

    [Fact]
    public void WhiteLabel_OnPink_WouldFail_WhichIsWhyTheLabelIsBlack() =>
        Assert.True(Contrast(BrandColors.White, BrandColors.Pink) < AA);

    [Fact]
    public void SecondaryButtonLabel_IsReadable_OnSecondaryGrey() =>
        Assert.True(Contrast(BrandColors.White, BrandColors.TextMuted) >= AA);
}
