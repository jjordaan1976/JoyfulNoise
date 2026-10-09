namespace Tutor.Client.Theme;

/// <summary>
/// Brand colours. Mirrors the CSS custom properties in wwwroot/theme.css
/// (ThemeParityTests keeps the two in step) and is documented in UI_SKILL.md section 2.
/// </summary>
public static class BrandColors
{
    // ── Neutrals ──────────────────────────────────────────────────────────────
    public const string White = "#ffffff";
    public const string Surface = "#ffffff";
    public const string Background = "#f7f7f7";
    public const string Raised = "#f0f0f0";
    public const string Border = "#e4e4e4";
    public const string Text = "#000000";
    public const string TextMuted = "#5f5f5f";
    public const string TextDim = "#8a8a8a";

    // ── Logo wave (sampled from the Joyful Noise logo) ────────────────────────
    public const string Mint = "#93d4c7";
    public const string Sage = "#b9cda4";
    public const string Amber = "#f6b669";
    public const string Coral = "#f79b83";
    public const string Pink = "#ef6ba3";
    public const string Orchid = "#aa77b8";

    // ── Derived from the logo ─────────────────────────────────────────────────
    /// <summary>Pink darkened so it can be used as text or an icon on any surface.</summary>
    public const string AccentDeep = "#c03276";

    /// <summary>Label colour on a pink fill. Black is 7.3:1; white would be only 2.9:1.</summary>
    public const string OnAccent = "#000000";

    // ── Status (never taken from the brand spectrum) ──────────────────────────
    public const string Success = "#2b7a2f";
    public const string Warn = "#a65700";
    public const string Danger = "#c62828";
}
