using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using TextSwap.UI;

namespace TextSwap.Core;

/// <summary>Consistent WinForms palette for Windows 10/11, including custom dropdowns.</summary>
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
        control.ForeColor = control.Tag is "muted" ? colors.Muted : colors.Text;
        control.BackColor = control switch
        {
            Form => colors.Background,
            Label or CheckBox => Color.Transparent,
            TextBoxBase => colors.Input,
            Button => colors.Button,
            _ when control.Tag is "section" or "header" => colors.Surface,
            Panel or TableLayoutPanel or FlowLayoutPanel or UserControl => colors.Background,
            _ => colors.Surface
        };

        switch (control)
        {
            case TextBoxBase tb:
                tb.BorderStyle = BorderStyle.None;
                break;
            case Button button:
                button.UseVisualStyleBackColor = false;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = colors.Border;
                button.FlatAppearance.MouseOverBackColor = colors.Hover;
                button.FlatAppearance.MouseDownBackColor = colors.Border;
                break;
            case LinkLabel link:
                link.LinkColor = colors.Link;
                link.ActiveLinkColor = colors.ActiveLink;
                link.VisitedLinkColor = colors.Link;
                break;
            case ThemePicker picker:
                picker.ApplyTheme(colors.Surface, colors.Text, colors.Hover);
                break;
        }

        foreach (Control child in control.Controls) ApplyControl(child, colors);
    }

    private static void ApplyMenu(ContextMenuStrip menu, Palette palette)
    {
        menu.BackColor = palette.Surface;
        menu.ForeColor = palette.Text;
        menu.Renderer = new ToolStripProfessionalRenderer(new MenuColors(palette.Surface, palette.Hover));
        foreach (ToolStripItem item in menu.Items)
        {
            item.BackColor = palette.Surface;
            item.ForeColor = palette.Text;
        }
    }

    private sealed class MenuColors(Color baseColor, Color hovered) : ProfessionalColorTable
    {
        public override Color MenuItemSelected => hovered;
        public override Color MenuItemBorder => hovered;
        public override Color ToolStripDropDownBackground => baseColor;
        public override Color ImageMarginGradientBegin => baseColor;
        public override Color ImageMarginGradientMiddle => baseColor;
        public override Color ImageMarginGradientEnd => baseColor;
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
        Color Text, Color Muted, Color Border, Color Hover, Color Link, Color ActiveLink)
    {
        public static Palette Dark { get; } = new(
            Color.FromArgb(28, 31, 37), Color.FromArgb(35, 39, 46),
            Color.FromArgb(25, 28, 34), Color.FromArgb(45, 50, 59),
            Color.FromArgb(231, 234, 240), Color.FromArgb(162, 172, 184),
            Color.FromArgb(65, 73, 85), Color.FromArgb(63, 77, 95),
            Color.FromArgb(132, 182, 245), Color.FromArgb(173, 208, 255));

        public static Palette Light { get; } = new(
            Color.FromArgb(245, 247, 250), Color.White,
            Color.FromArgb(250, 251, 253), Color.FromArgb(236, 239, 244),
            Color.FromArgb(36, 41, 50), Color.FromArgb(101, 110, 123),
            Color.FromArgb(202, 210, 222), Color.FromArgb(214, 225, 239),
            Color.FromArgb(29, 91, 172), Color.FromArgb(13, 68, 149));
    }
}
