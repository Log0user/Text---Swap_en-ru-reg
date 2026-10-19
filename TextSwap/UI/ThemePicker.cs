using System.Drawing;

namespace TextSwap.UI;

/// <summary>A keyboard-accessible, fully themed alternative to the WinForms native combo box.</summary>
public sealed class ThemePicker : UserControl
{
    private readonly Button _button;
    private readonly ContextMenuStrip _list = new() { ShowImageMargin = false };
    private readonly string[] _choices;
    private int _index = -1;

    public event EventHandler? SelectedIndexChanged;

    public int SelectedIndex
    {
        get => _index;
        set
        {
            if (value < 0 || value >= _choices.Length) throw new ArgumentOutOfRangeException(nameof(value));
            if (_index == value) return;
            _index = value;
            _button.Text = _choices[value] + "  ▾";
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public ThemePicker(string[] choices)
    {
        if (choices.Length == 0) throw new ArgumentException("Choose at least one option", nameof(choices));
        _choices = choices;
        Height = 32;
        MinimumSize = new Size(96, 32);
        Margin = new Padding(3, 3, 9, 3);
        TabStop = false;
        _button = new Button { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, TabStop = true };
        _button.Click += (_, _) => OpenDropdown();
        _button.KeyDown += (_, e) =>
        {
            if (e.KeyCode is Keys.Down or Keys.Up)
            {
                SelectedIndex = (SelectedIndex + (e.KeyCode == Keys.Down ? 1 : _choices.Length - 1)) % _choices.Length;
                e.Handled = e.SuppressKeyPress = true;
            }
        };
        Controls.Add(_button);
        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            var item = new ToolStripMenuItem(choices[i]);
            item.Click += (_, _) => SelectedIndex = index;
            _list.Items.Add(item);
        }
        SelectedIndex = 0;
    }

    private void OpenDropdown()
    {
        foreach (ToolStripMenuItem item in _list.Items)
            item.Checked = _list.Items.IndexOf(item) == SelectedIndex;
        _list.MinimumSize = new Size(Width, 0);
        _list.Show(_button, new Point(0, _button.Height));
    }

    public void ApplyTheme(Color surface, Color foreground, Color border)
    {
        _list.BackColor = surface;
        _list.ForeColor = foreground;
        _list.Renderer = new ToolStripProfessionalRenderer(new MenuColors(surface, border));
        foreach (ToolStripItem item in _list.Items)
        {
            item.BackColor = surface;
            item.ForeColor = foreground;
        }
    }

    private sealed class MenuColors(Color surface, Color highlight) : ProfessionalColorTable
    {
        public override Color MenuItemSelected => highlight;
        public override Color MenuItemBorder => highlight;
        public override Color ToolStripDropDownBackground => surface;
        public override Color ImageMarginGradientBegin => surface;
        public override Color ImageMarginGradientMiddle => surface;
        public override Color ImageMarginGradientEnd => surface;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _list.Dispose();
        base.Dispose(disposing);
    }
}
