using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace TextSwap.Core;

/// <summary>
/// Applies the application's own light/dark palette. Unlike native WinForms dark mode,
/// this also works on Windows 10. "System" follows the per-user app theme setting.
/// </summary>
public static class ThemeManager
{
    private const int DarkTitleBarAttribute = 20;
    private const string ThemeRegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    [DllImport("dwmapi.dll", PreserveSig = true)]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    public static bool IsDark(AppTheme theme)
    {
        if (SystemInformation.HighContrast) return false;
        if (theme == AppTheme.Dark) return true;
        if (theme == AppTheme.Light) return false;

        try
        {
            return Registry.GetValue(ThemeRegistryKey, "AppsUseLightTheme", 1) is int value && value == 0;
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or System.Security.SecurityException or IOException)
        {
            return false;
        }
    }

    public static void Apply(Form form, AppTheme theme, ContextMenuStrip? menu = null)
    {
        var dark = IsDark(theme);
        var palette = dark ? Palette.Dark : Palette.Light;
        form.SuspendLayout();
        try
        {
            ApplyControl(form, palette);
            if (menu is not null) ApplyMenu(menu, palette);
            if (form.IsHandleCreated) ApplyTitleBar(form.Handle, dark);
        }
        finally { form.ResumeLayout(true); }
    }

    private static void ApplyControl(Control control, Palette colors)
    {
        control.BackColor = control is Form or Panel or TableLayoutPanel or FlowLayoutPanel
            ? colors.Background : colors.Surface;
        control.ForeColor = colors.Text;

        switch (control)
        {
            case TextBoxBase:
                control.BackColor = colors.Input;
                break;
            case ComboBox combo:
                combo.BackColor = colors.Input;
                break;
            case Button button:
                button.UseVisualStyleBackColor = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = colors.Border;
                button.BackColor = colors.Button;
                break;
            case LinkLabel link:
                link.LinkColor = colors.Link;
                link.ActiveLinkColor = colors.ActiveLink;
                link.VisitedLinkColor = colors.Link;
                break;
        }

        foreach (Control child in control.Controls) ApplyControl(child, colors);
    }

    private static void ApplyMenu(ContextMenuStrip menu, Palette palette)
    {
        menu.BackColor = palette.Surface;
        menu.ForeColor = palette.Text;
        foreach (ToolStripItem item in menu.Items)
        {
            item.BackColor = palette.Surface;
            item.ForeColor = palette.Text;
        }
    }

    private static void ApplyTitleBar(IntPtr handle, bool dark)
    {
        try
        {
            int enabled = dark ? 1 : 0;
            _ = DwmSetWindowAttribute(handle, DarkTitleBarAttribute, ref enabled, sizeof(int));
        }
        catch (DllNotFoundException) { }
        catch (EntryPointNotFoundException) { }
    }

    private sealed record Palette(Color Background, Color Surface, Color Input, Color Button,
        Color Text, Color Border, Color Link, Color ActiveLink)
    {
        public static Palette Dark { get; } = new(
            Color.FromArgb(30, 33, 40), Color.FromArgb(39, 43, 51),
            Color.FromArgb(26, 29, 35), Color.FromArgb(53, 58, 68),
            Color.FromArgb(238, 241, 247), Color.FromArgb(99, 106, 121),
            Color.FromArgb(122, 178, 255), Color.FromArgb(165, 204, 255));

        public static Palette Light { get; } = new(
            Color.FromArgb(249, 250, 252), Color.White,
            Color.White, Color.FromArgb(242, 244, 248),
            Color.FromArgb(28, 32, 41), Color.FromArgb(191, 196, 205),
            Color.FromArgb(18, 95, 197), Color.FromArgb(7, 63, 157));
    }
}
