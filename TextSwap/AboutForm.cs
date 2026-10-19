using System.Diagnostics;
using System.Drawing;
using TextSwap.Core;
using TextSwap.Updates;

namespace TextSwap;

public sealed class AboutForm : Form
{
    private readonly Label _message = new()
    {
        Dock = DockStyle.Fill, Text = "Обновления проверяются только по запросу.",
        AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft, Tag = "muted"
    };
    private readonly Label _backupInfo = new()
    {
        Dock = DockStyle.Fill, AutoEllipsis = true, TextAlign = ContentAlignment.MiddleLeft, Tag = "muted"
    };
    private readonly Button _check = new() { Text = "Проверить обновление", AutoSize = true };
    private readonly Button _update = new() { Text = "Обновить", AutoSize = true, Enabled = false };
    private readonly Button _switch = new() { AutoSize = true, Enabled = false };
    private readonly Button _delete = new() { Text = "Удалить копию", AutoSize = true, Enabled = false };
    private readonly Button _expand = new() { Text = "Резервная копия  ▸", AutoSize = true };
    private readonly TableLayoutPanel _layout;
    private readonly AppTheme _theme;
    private AvailableUpdate? _available;
    private LocalBackupInfo? _backup;
    private bool _expanded;

    public AboutForm(AppTheme theme)
    {
        _theme = theme;
        Text = "О программе — Text Swap";
        ClientSize = new Size(640, 335);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 9.5f);

        _layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(18, 14, 18, 12),
            ColumnCount = 1, RowCount = 7
        };
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        _layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
        Controls.Add(_layout);

        _layout.Controls.Add(new Label
        {
            Text = "Text Swap — версия " + UpdateService.CurrentVersion.ToString(3),
            Font = new Font("Segoe UI Semibold", 14f, FontStyle.Bold),
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);
        _layout.Controls.Add(new Label
        {
            Text = "Конвертер раскладки RU ⇄ EN · Бесплатное открытое ПО · AGPL-3.0",
            Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        var link = new LinkLabel { Text = UpdateService.RepositoryUrl, Dock = DockStyle.Fill, AutoEllipsis = true };
        link.LinkClicked += (_, _) => OpenUrl(UpdateService.RepositoryUrl);
        _layout.Controls.Add(link, 0, 2);
        _layout.Controls.Add(_message, 0, 3);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = false, WrapContents = false };
        _check.Click += async (_, _) => await CheckAsync();
        _update.Click += async (_, _) => await InstallAsync();
        buttons.Controls.Add(_check);
        buttons.Controls.Add(_update);
        _layout.Controls.Add(buttons, 0, 4);

        _expand.Click += (_, _) => ToggleBackup();
        var toggle = new FlowLayoutPanel { Dock = DockStyle.Fill };
        toggle.Controls.Add(_expand);
        _layout.Controls.Add(toggle, 0, 5);

        var details = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Tag = "section", Padding = new Padding(12, 5, 12, 8),
            RowCount = 2, ColumnCount = 1
        };
        details.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
        details.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
        details.Controls.Add(_backupInfo, 0, 0);
        var backupActions = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, AutoScroll = false };
        _switch.Click += (_, _) => SwitchVersion();
        _delete.Click += (_, _) => DeleteBackup();
        backupActions.Controls.Add(_switch);
        backupActions.Controls.Add(_delete);
        details.Controls.Add(backupActions, 0, 1);
        _layout.Controls.Add(details, 0, 6);
        details.Visible = false;
        RefreshBackup();
        ThemeManager.Apply(this, _theme);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ThemeManager.Apply(this, _theme);
    }

    private void ToggleBackup()
    {
        _expanded = !_expanded;
        _layout.SuspendLayout();
        try
        {
            _expand.Text = _expanded ? "Резервная копия  ▾" : "Резервная копия  ▸";
            _layout.RowStyles[6].Height = _expanded ? 105 : 0;
            _layout.GetControlFromPosition(0, 6)!.Visible = _expanded;
            ClientSize = new Size(640, _expanded ? 440 : 335);
            if (_expanded) RefreshBackup();
        }
        finally { _layout.ResumeLayout(true); }
    }

    private void RefreshBackup()
    {
        _backup = null;
        _switch.Enabled = false;
        _delete.Enabled = false;
        string? current = Environment.ProcessPath;
        if (current is null || !current.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
        {
            _backupInfo.Text = "Резервная копия доступна только в собранном EXE.";
            return;
        }
        string path = LocalBackup.GetPath(current);
        if (!File.Exists(path))
        {
            _backupInfo.Text = "Локальная резервная копия отсутствует.";
            return;
        }
        _delete.Enabled = true;
        try
        {
            _backup = LocalBackup.Inspect(current);
            if (_backup is null)
            {
                _backupInfo.Text = "Версию файла определить не удалось. Переключение недоступно.";
                return;
            }
            _backupInfo.Text = $"В локальной копии: {_backup.Version.ToString(3)} · {_backup.Bytes / 1048576d:F1} МБ. Интернет не нужен.";
            _switch.Text = LocalBackup.SwitchLabel(UpdateService.CurrentVersion, _backup.Version);
            _switch.Enabled = true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            _backupInfo.Text = "Не удалось прочитать сведения о копии: " + ex.Message;
        }
    }

    private void SwitchVersion()
    {
        RefreshBackup();
        if (_backup is null) return;
        string action = LocalBackup.SwitchLabel(UpdateService.CurrentVersion, _backup.Version);
        if (MessageBox.Show(this,
                action + "? Программа перезапустится, а нынешняя версия сохранится как .previous. " +
                "Настройки останутся. В старых версиях эта кнопка может отсутствовать.",
                "Переключение версии", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        try
        {
            _switch.Enabled = false;
            UpdateService.LaunchRollback();
            Application.Exit();
        }
        catch (Exception ex)
        {
            _message.Text = "Не получилось переключить версию: " + ex.Message;
            RefreshBackup();
        }
    }

    private void DeleteBackup()
    {
        string? current = Environment.ProcessPath;
        if (current is null) return;
        string file = LocalBackup.GetPath(current);
        if (!File.Exists(file)) { RefreshBackup(); return; }
        if (MessageBox.Show(this,
            "Удалить только локальный файл .previous? Это освободит место, но переключение версии " +
            "без скачивания станет недоступно. Настройки и текущий EXE не изменятся.",
            "Удаление резервной копии", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        try
        {
            LocalBackup.Delete(current);
            RefreshBackup();
            _message.Text = "Резервная копия удалена.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _message.Text = "Не удалось удалить резервную копию: " + ex.Message;
        }
    }

    private static void OpenUrl(string url) =>
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

    private async Task CheckAsync()
    {
        _available = null;
        _check.Enabled = _update.Enabled = false;
        _message.Text = "Проверяю официальный GitHub Releases…";
        try
        {
            _available = await UpdateService.CheckAsync();
            if (IsDisposed) return;
            if (_available is null) _message.Text = "Обновлений нет (или ещё не опубликован полноценный GitHub Release).";
            else
            {
                _message.Text = $"Доступна версия {_available.Version}. Можно скачать и установить.";
                _update.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            if (!IsDisposed) _message.Text = "Ошибка проверки: " + ex.Message;
        }
        finally { if (!IsDisposed) _check.Enabled = true; }
    }

    private async Task InstallAsync()
    {
        if (_available is null) return;
        var answer = MessageBox.Show(this,
            "Скачать проверенную по SHA-256 версию " + _available.Version +
            " и перезапустить Text Swap? Настройки сохранятся.", "Обновление",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes) return;
        _check.Enabled = _update.Enabled = _switch.Enabled = _delete.Enabled = false;
        var progress = new Progress<int>(value => { if (!IsDisposed) _message.Text = $"Загрузка: {value}%"; });
        try
        {
            string file = await UpdateService.DownloadVerifiedAsync(_available, progress);
            if (IsDisposed) return;
            _message.Text = "Файл проверен. Перезапускаю приложение…";
            UpdateService.LaunchInstaller(file);
            Application.Exit();
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
            {
                _message.Text = "Обновление не выполнено: " + ex.Message;
                _update.Enabled = true;
            }
        }
        finally
        {
            if (!IsDisposed)
            {
                _check.Enabled = true;
                RefreshBackup();
            }
        }
    }
}
