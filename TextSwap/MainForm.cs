using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using TextSwap.Core;
using TextSwap.Windows;
using TextSwap.UI;
using TextSwap.Updates;

namespace TextSwap;

public sealed class MainForm : Form
{
    private const int ClipboardHotkeyId = 100;
    private const int SelectionHotkeyId = 101;

    private readonly AppSettings _settings;
    private readonly ThemePicker _mode = new(
        ["Авто: преобладающий алфавит", "По фрагментам (эксперимент)", "EN → RU", "RU → EN"]) { Width = 240 };
    private readonly ThemePicker _theme = new(["Как в Windows", "Светлая", "Тёмная"]) { Width = 145 };
    private bool _currentDark;
    private readonly TextBox _input = new() { Multiline = true, ScrollBars = ScrollBars.Both, WordWrap = true, Dock = DockStyle.Fill };
    private readonly TextBox _output = new() { Multiline = true, ScrollBars = ScrollBars.Both, WordWrap = true, Dock = DockStyle.Fill, ReadOnly = true };
    private readonly Label _status = new() { Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft };
    private readonly NotifyIcon _tray;
    private bool _registeredF9;
    private bool _registeredCtrlF9;
    private bool _exitRequested;
    private bool _busySelection;
    private bool _initialized;

    public MainForm(AppSettings settings)
    {
        _settings = settings;
        Text = "Text Swap " + UpdateService.CurrentVersion.ToString(3) + " — смена раскладки";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(735, 540);
        Size = new Size(935, 690);
        Font = new Font("Segoe UI", 9.5f);
        Icon = SystemIcons.Application;
        BuildInterface();

        var menu = new ContextMenuStrip();
        menu.Items.Add("Открыть", null, (_, _) => ShowWindow());
        menu.Items.Add("Преобразовать буфер (F9)", null, (_, _) => ConvertClipboardAndShow());
        menu.Items.Add("О программе", null, (_, _) => ShowAbout());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Выход", null, (_, _) => ExitApplication());
        _tray = new NotifyIcon
        {
            Icon = Icon,
            Text = "Text Swap " + UpdateService.CurrentVersion.ToString(3),
            ContextMenuStrip = menu,
            Visible = true
        };
        _tray.DoubleClick += (_, _) => ConvertClipboardAndShow();
        _initialized = true;
        ApplyTheme();
    }

    private void BuildInterface()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14),
            ColumnCount = 1,
            RowCount = 7
        };
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 43));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 78));
        main.RowStyles.Add(new RowStyle(SizeType.Absolute, 27));
        Controls.Add(main);

        var header = new Panel { Dock = DockStyle.Fill, Tag = "header", Padding = new Padding(12, 0, 10, 0) };
        header.Controls.Add(new Label
        {
            Text = "Text Swap    ·    RU ⇄ EN",
            Font = new Font("Segoe UI Semibold", 13.5f, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        });
        main.Controls.Add(header, 0, 0);

        var toolbar = CreateFlow();
        toolbar.Controls.Add(new Label { Text = "Направление:", AutoSize = true, Margin = new Padding(0, 9, 9, 0) });
        _mode.SelectedIndex = (int)_settings.Mode;
        _mode.SelectedIndexChanged += (_, _) =>
        {
            if (!_initialized) return;
            _settings.Mode = (ConversionMode)_mode.SelectedIndex;
            SaveSettings();
        };
        toolbar.Controls.Add(_mode);
        toolbar.Controls.Add(new Label { Text = "Тема:", AutoSize = true, Margin = new Padding(8, 9, 4, 0) });
        _theme.SelectedIndex = (int)_settings.Theme;
        _theme.SelectedIndexChanged += (_, _) =>
        {
            if (!_initialized) return;
            _settings.Theme = (AppTheme)_theme.SelectedIndex;
            SaveSettings();
            ApplyTheme();
        };
        toolbar.Controls.Add(_theme);
        toolbar.Controls.Add(MakeButton("Преобразовать", (_, _) => ConvertInput()));
        toolbar.Controls.Add(MakeButton("О программе", (_, _) => ShowAbout()));
        main.Controls.Add(toolbar, 0, 1);

        main.Controls.Add(MakeTextGroup("Исходный текст", _input), 0, 2);
        main.Controls.Add(MakeTextGroup("Результат", _output), 0, 3);

        var actions = CreateFlow();
        actions.Controls.Add(MakeButton("Из буфера", (_, _) => LoadClipboard(convert: false)));
        actions.Controls.Add(MakeButton("В буфер", (_, _) => CopyOutput()));
        actions.Controls.Add(MakeButton("Результат → ввод", (_, _) => { _input.Text = _output.Text; _output.Clear(); }));
        actions.Controls.Add(MakeButton("ПРОПИСНЫЕ", (_, _) => _output.Text = _input.Text.ToUpperInvariant()));
        actions.Controls.Add(MakeButton("строчные", (_, _) => _output.Text = _input.Text.ToLowerInvariant()));
        main.Controls.Add(actions, 0, 4);

        var options = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
        options.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        options.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        var row1 = CreateFlow(scroll: false);
        row1.Controls.Add(Option("Копировать результат", _settings.AutoCopyResult, value => _settings.AutoCopyResult = value));
        row1.Controls.Add(Option("Сворачивать в трей", _settings.MinimizeToTray, value => _settings.MinimizeToTray = value));
        row1.Controls.Add(Option("Закрывать в трей", _settings.CloseToTray, value => _settings.CloseToTray = value));
        var row2 = CreateFlow(scroll: false);
        row2.Controls.Add(Option("Запускать в трее", _settings.StartMinimized, value => _settings.StartMinimized = value));
        row2.Controls.Add(Option("Глобальные F9 / Ctrl+F9", _settings.EnableGlobalHotkeys, value =>
        {
            _settings.EnableGlobalHotkeys = value;
            if (IsHandleCreated) RefreshHotkeys();
        }));
        options.Controls.Add(row1, 0, 0);
        options.Controls.Add(row2, 0, 1);
        main.Controls.Add(options, 0, 5);

        _status.Text = "F9 — буфер → результат; Ctrl+F9 — преобразовать выделенное и вставить.";
        _status.Tag = "muted";
        main.Controls.Add(_status, 0, 6);
    }

    private static FlowLayoutPanel CreateFlow(bool scroll = false) => new()
    {
        Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight,
        WrapContents = false, AutoScroll = scroll,
        Margin = new Padding(0, 3, 0, 3)
    };

    private static Button MakeButton(string caption, EventHandler action)
    {
        var button = new Button { Text = caption, AutoSize = true, MinimumSize = new Size(85, 29), Margin = new Padding(3, 2, 8, 2) };
        button.Click += action;
        return button;
    }

    private CheckBox Option(string caption, bool isChecked, Action<bool> setter)
    {
        var checkbox = new CheckBox { Text = caption, Checked = isChecked, AutoSize = true, Margin = new Padding(5, 6, 16, 6) };
        checkbox.CheckedChanged += (_, _) =>
        {
            setter(checkbox.Checked);
            SaveSettings();
        };
        return checkbox;
    }

    private static Control MakeTextGroup(string caption, TextBox box)
    {
        var section = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Tag = "section", Padding = new Padding(12, 4, 12, 10),
            ColumnCount = 1, RowCount = 2, Margin = new Padding(0, 3, 0, 3)
        };
        section.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        section.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        section.Controls.Add(new Label
        {
            Text = caption, Dock = DockStyle.Fill,
            Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        box.BorderStyle = BorderStyle.None;
        box.Margin = new Padding(0, 0, 0, 0);
        section.Controls.Add(box, 0, 1);
        return section;
    }

    private void SaveSettings()
    {
        try { _settings.Save(); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            SetStatus("Не удалось сохранить настройки: " + ex.Message);
        }
    }

    private void SetStatus(string message) => _status.Text = message;

    private void ConvertInput(bool copy = false)
    {
        _output.Text = LayoutConverter.Convert(_input.Text, _settings.Mode);
        if (copy || _settings.AutoCopyResult) CopyOutput();
        else SetStatus("Преобразовано.");
    }

    private void CopyOutput()
    {
        try
        {
            if (_output.Text.Length > 0)
            {
                Clipboard.SetText(_output.Text);
                SetStatus("Результат скопирован в буфер обмена.");
            }
            else SetStatus("Результат пуст.");
        }
        catch (ExternalException ex) { SetStatus("Буфер обмена занят: " + ex.Message); }
    }

    private void LoadClipboard(bool convert)
    {
        try
        {
            if (!Clipboard.ContainsText()) { SetStatus("В буфере обмена нет текста."); return; }
            _input.Text = Clipboard.GetText();
            if (convert) ConvertInput(copy: true);
            else SetStatus("Текст загружен из буфера.");
        }
        catch (ExternalException ex) { SetStatus("Буфер обмена занят: " + ex.Message); }
    }

    private void ConvertClipboardAndShow()
    {
        ShowWindow();
        LoadClipboard(convert: true);
    }

    private void ShowWindow()
    {
        Show();
        WindowState = FormWindowState.Normal;
        ShowInTaskbar = true;
        Activate();
    }

    private void ShowAbout()
    {
        using var about = new AboutForm(_settings.Theme);
        about.ShowDialog(this);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_settings.StartMinimized) BeginInvoke(new Action(HideToTray));
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_initialized && WindowState == FormWindowState.Minimized && _settings.MinimizeToTray)
            HideToTray();
    }

    private void HideToTray()
    {
        Hide();
        ShowInTaskbar = false;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_exitRequested && e.CloseReason == CloseReason.UserClosing && _settings.CloseToTray)
        {
            e.Cancel = true;
            HideToTray();
            return;
        }
        base.OnFormClosing(e);
    }

    private void ExitApplication()
    {
        _exitRequested = true;
        Close();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (_initialized) ApplyTheme();
        RefreshHotkeys();
    }

    private void RefreshHotkeys()
    {
        if (!IsHandleCreated) return;
        if (_registeredF9) NativeMethods.UnregisterHotKey(Handle, ClipboardHotkeyId);
        if (_registeredCtrlF9) NativeMethods.UnregisterHotKey(Handle, SelectionHotkeyId);
        _registeredF9 = _registeredCtrlF9 = false;
        if (!_settings.EnableGlobalHotkeys) return;

        _registeredF9 = NativeMethods.RegisterHotKey(Handle, ClipboardHotkeyId,
            NativeMethods.MOD_NOREPEAT, NativeMethods.VK_F9);
        _registeredCtrlF9 = NativeMethods.RegisterHotKey(Handle, SelectionHotkeyId,
            NativeMethods.MOD_NOREPEAT | NativeMethods.MOD_CONTROL, NativeMethods.VK_F9);
        if (!_registeredF9 || !_registeredCtrlF9)
            SetStatus("Не все горячие клавиши доступны: возможно, их использует другая программа.");
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        if (_registeredF9) NativeMethods.UnregisterHotKey(Handle, ClipboardHotkeyId);
        if (_registeredCtrlF9) NativeMethods.UnregisterHotKey(Handle, SelectionHotkeyId);
        _registeredF9 = _registeredCtrlF9 = false;
        base.OnHandleDestroyed(e);
    }

    private void ApplyTheme()
    {
        _currentDark = ThemeManager.IsDark(_settings.Theme);
        ThemeManager.Apply(this, _settings.Theme, _tray.ContextMenuStrip);
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        if (_settings.Theme == AppTheme.System && _currentDark != ThemeManager.IsDark(AppTheme.System))
            ApplyTheme();
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == NativeMethods.WM_HOTKEY)
        {
            int id = message.WParam.ToInt32();
            if (id == ClipboardHotkeyId) ConvertClipboardAndShow();
            else if (id == SelectionHotkeyId) _ = ConvertSelectionAsync();
        }
        base.WndProc(ref message);
        // Settings change, including the system light/dark application theme.
        if ((message.Msg == 0x001A || message.Msg == 0x031A) && _initialized &&
            _settings.Theme == AppTheme.System && _currentDark != ThemeManager.IsDark(AppTheme.System))
            ApplyTheme();
    }

    private async Task ConvertSelectionAsync()
    {
        if (_busySelection) return;
        _busySelection = true;
        try
        {
            // Wait until the user releases the Ctrl+F9 combination.
            await Task.Delay(180);
            uint before = NativeMethods.GetClipboardSequenceNumber();
            if (!NativeMethods.SendControlShortcut(NativeMethods.VK_C))
            {
                SetStatus("Windows не позволила отправить Ctrl+C.");
                return;
            }
            await Task.Delay(200);
            if (NativeMethods.GetClipboardSequenceNumber() == before || !Clipboard.ContainsText())
            {
                SetStatus("Нет выделенного текста или копирование недоступно.");
                return;
            }
            string selected = Clipboard.GetText();
            if (selected.Length == 0) return;
            string converted = LayoutConverter.Convert(selected, _settings.Mode);
            if (converted == selected) { SetStatus("Текст не изменился."); return; }

            _input.Text = selected;
            _output.Text = converted;
            Clipboard.SetText(converted);
            await Task.Delay(80);
            if (!NativeMethods.SendControlShortcut(NativeMethods.VK_V))
                SetStatus("Не удалось вставить: результат остаётся в буфере обмена.");
            else
                SetStatus("Выделенный текст заменён; результат остался в буфере обмена.");
        }
        catch (ExternalException ex) { SetStatus("Буфер обмена занят: " + ex.Message); }
        catch (Exception ex) { SetStatus("Не удалось заменить выделение: " + ex.Message); }
        finally { _busySelection = false; }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tray.Visible = false;
            _tray.ContextMenuStrip?.Dispose();
            _tray.Dispose();
        }
        base.Dispose(disposing);
    }
}
