using System.Text.RegularExpressions;

namespace Tutor.Client.Tests;

/// <summary>
/// Status chips (pastel background + coloured text) are defined once in theme.css.
/// Each pairing must stay readable.
/// </summary>
public class ThemeChipContrastTests
{
    private static Dictionary<string, string> Tokens()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "theme.css"));
        return Regex.Matches(css, @"--([a-z0-9-]+)\s*:\s*(#[0-9a-fA-F]{6})\s*;")
                    .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value.ToLowerInvariant());
    }

    public static TheoryData<string> Chips => new()
    {
        "scheduled", "completed", "forfeited", "cancelled", "rescheduled", "pending", "paid", "overdue", "void"
    };

    [Theory, MemberData(nameof(Chips))]
    public void ChipText_IsReadable_OnItsBackground(string chip)
    {
        var tokens = Tokens();
        var bg = $"chip-{chip}-bg";
        var fg = $"chip-{chip}-fg";

        Assert.True(tokens.ContainsKey(bg), $"theme.css does not define --{bg}");
        Assert.True(tokens.ContainsKey(fg), $"theme.css does not define --{fg}");
        Assert.True(ThemeContrastTests.Contrast(tokens[fg], tokens[bg]) >= 4.5,
            $"--{fg} on --{bg} is only {ThemeContrastTests.Contrast(tokens[fg], tokens[bg]):N2}:1");
    }
}
