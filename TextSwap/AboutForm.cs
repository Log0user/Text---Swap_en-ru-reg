using System.Diagnostics;
using TextSwap.Updates;

namespace TextSwap;

public sealed class AboutForm : Form
{
    private readonly Label _message = new()
    {
        Dock = DockStyle.Fill, Text = "Обновления проверяются только по запросу.",
        AutoEllipsis = true, TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    };
    private readonly Button _check = new() { Text = "Проверить обновление", AutoSize = true };
    private readonly Button _update = new() { Text = "Обновить", AutoSize = true, Enabled = false };
    private AvailableUpdate? _available;

    public AboutForm()
    {
        Text = "О программе — Text Swap";
        Size = new System.Drawing.Size(540, 295);
        MinimumSize = Size;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Font = new System.Drawing.Font("Segoe UI", 9.5f);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(16),
            ColumnCount = 1, RowCount = 5
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        Controls.Add(layout);

        var caption = new Label
        {
            Text = "Text Swap — версия " + UpdateService.CurrentVersion.ToString(3),
            Font = new System.Drawing.Font("Segoe UI Semibold", 14f, System.Drawing.FontStyle.Bold),
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(caption, 0, 0);
        var info = new Label
        {
            Text = "Конвертер раскладки RU ⇄ EN. Бесплатно, локально, открытый код. AGPL-3.0.",
            Dock = DockStyle.Fill
        };
        layout.Controls.Add(info, 0, 1);

        var link = new LinkLabel { Text = UpdateService.RepositoryUrl, Dock = DockStyle.Fill, AutoEllipsis = true };
        link.LinkClicked += (_, _) => OpenUrl(UpdateService.RepositoryUrl);
        layout.Controls.Add(link, 0, 2);
        layout.Controls.Add(_message, 0, 3);

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        _check.Click += async (_, _) => await CheckAsync();
        _update.Click += async (_, _) => await InstallAsync();
        buttons.Controls.Add(_check);
        buttons.Controls.Add(_update);
        layout.Controls.Add(buttons, 0, 4);
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
        _check.Enabled = _update.Enabled = false;
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
        finally { if (!IsDisposed) _check.Enabled = true; }
    }
}
