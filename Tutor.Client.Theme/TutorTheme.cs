using MudBlazor;

namespace Tutor.Client.Theme;

/// <summary>The MudBlazor theme shared by Tutor.Web, Tutor.StudentPortal and Tutor.AccountHolderPortal.</summary>
public static class TutorTheme
{
    private static readonly string[] Font = ["Inter", "Segoe UI", "system-ui", "Roboto", "sans-serif"];

    public static MudTheme Light { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = BrandColors.Pink,
            PrimaryContrastText = BrandColors.OnAccent,
            // Secondary is the app's muted-caption colour (Color.Secondary on MudText), so it stays neutral.
            Secondary = BrandColors.TextMuted,
            SecondaryContrastText = BrandColors.White,
            Background = BrandColors.Background,
            Surface = BrandColors.Surface,
            AppbarBackground = BrandColors.Surface,
            AppbarText = BrandColors.Text,
            DrawerBackground = BrandColors.Surface,
            DrawerText = BrandColors.Text,
            TextPrimary = BrandColors.Text,
            TextSecondary = BrandColors.TextMuted,
            Divider = BrandColors.Border,
            TableLines = BrandColors.Border,
            Success = BrandColors.Success,
            Warning = BrandColors.Warn,
            Error = BrandColors.Danger,
        },
        Typography = new Typography
        {
            Default = new Default { FontFamily = Font },
            H1 = new H1 { FontFamily = Font },
            H2 = new H2 { FontFamily = Font },
            H3 = new H3 { FontFamily = Font },
            H4 = new H4 { FontFamily = Font },
            H5 = new H5 { FontFamily = Font },
            H6 = new H6 { FontFamily = Font },
            Subtitle1 = new Subtitle1 { FontFamily = Font },
            Subtitle2 = new Subtitle2 { FontFamily = Font },
            Body1 = new Body1 { FontFamily = Font },
            Body2 = new Body2 { FontFamily = Font },
            Button = new Button { FontFamily = Font },
            Caption = new Caption { FontFamily = Font },
            Overline = new Overline { FontFamily = Font },
        },
    };
}

/// <summary>Static web asset paths served by this project (see wwwroot/).</summary>
public static class ThemeAssets
{
    public const string LogoFull = "_content/Tutor.Client.Theme/images/logo-full.png";
    public const string LogoWave = "_content/Tutor.Client.Theme/images/logo-wave.png";
}
