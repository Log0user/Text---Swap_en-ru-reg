using System.Diagnostics;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using TextSwap.Core;

namespace TextSwap.Updates;

public sealed record AvailableUpdate(Version Version, string Tag, string AssetUrl, string HashUrl, string ReleaseUrl);

/// <summary>Checks only the original repository's public GitHub Releases.</summary>
public static class UpdateService
{
    public const string RepositoryUrl = "https://github.com/Log0user/Text---Swap_en-ru-reg";
    private const string ApiUrl = "https://api.github.com/repos/Log0user/Text---Swap_en-ru-reg/releases/latest";
    private const string AssetName = "TextSwap-win-x64.exe";
    private const long MaxExecutableSize = 250L * 1024 * 1024;

    private static readonly HttpClient Client = CreateClient();
    public static Version CurrentVersion => typeof(UpdateService).Assembly.GetName().Version ?? new Version(2, 0, 0);

    private static HttpClient CreateClient()
    {
        var client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("TextSwap-Updater/2.0");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        return client;
    }

    public static async Task<AvailableUpdate?> CheckAsync(CancellationToken token = default)
    {
        using var reply = await Client.GetAsync(ApiUrl, token);
        if (reply.StatusCode == System.Net.HttpStatusCode.NotFound) return null; // No Releases yet.
        reply.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await reply.Content.ReadAsStreamAsync(token));
        var root = document.RootElement;
        if (root.GetProperty("draft").GetBoolean() || root.GetProperty("prerelease").GetBoolean()) return null;

        var tag = root.GetProperty("tag_name").GetString() ?? "";
        string clean = tag.TrimStart('v', 'V');
        if (!Version.TryParse(clean, out var latest) || latest <= CurrentVersion) return null;

        var assets = root.GetProperty("assets");
        string? FindUrl(string name)
        {
            foreach (var asset in assets.EnumerateArray())
                if (asset.GetProperty("name").GetString() == name)
                    return asset.GetProperty("browser_download_url").GetString();
            return null;
        }

        var exeUrl = FindUrl(AssetName);
        var hashUrl = FindUrl(AssetName + ".sha256");
        if (exeUrl is null || hashUrl is null) throw new InvalidDataException(
            "Релиз найден, но отсутствуют TextSwap-win-x64.exe или его SHA-256 файл.");

        // Only install release assets published in the exact repository.
        if (!IsTrustedAssetUrl(exeUrl) || !IsTrustedAssetUrl(hashUrl))
            throw new InvalidDataException("Некорректный адрес файла обновления.");
        string releaseUrl = $"{RepositoryUrl}/releases/tag/{Uri.EscapeDataString(tag)}";
        return new AvailableUpdate(latest, tag, exeUrl, hashUrl, releaseUrl);
    }

    private static bool IsTrustedAssetUrl(string candidate) =>
        Uri.TryCreate(candidate, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps &&
        uri.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) &&
        uri.AbsolutePath.StartsWith("/Log0user/Text---Swap_en-ru-reg/releases/download/", StringComparison.Ordinal);

    public static async Task<string> DownloadVerifiedAsync(AvailableUpdate update,
        IProgress<int>? progress = null, CancellationToken token = default)
    {
        string checksum = await Client.GetStringAsync(update.HashUrl, token);
        var digest = Regex.Match(checksum, @"\b[0-9a-fA-F]{64}\b");
        if (!digest.Success) throw new InvalidDataException("Отсутствует корректная сумма SHA-256.");

        string folder = Path.Combine(Path.GetTempPath(), "TextSwap-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(folder);
        string downloadedPath = Path.Combine(folder, "TextSwap.exe");

        try
        {
            using var response = await Client.GetAsync(update.AssetUrl, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentLength > MaxExecutableSize)
                throw new InvalidDataException("Слишком большой файл обновления.");
            long? size = response.Content.Headers.ContentLength;
            long bytes = 0;
            await using (var source = await response.Content.ReadAsStreamAsync(token))
            await using (var destination = new FileStream(downloadedPath, FileMode.CreateNew,
                FileAccess.Write, FileShare.None, 65536, useAsync: true))
            {
                byte[] buffer = new byte[65536];
                int count;
                while ((count = await source.ReadAsync(buffer, token)) != 0)
                {
                    bytes += count;
                    if (bytes > MaxExecutableSize) throw new InvalidDataException("Файл превышает допустимый размер.");
                    await destination.WriteAsync(buffer.AsMemory(0, count), token);
                    if (size is > 0) progress?.Report((int)Math.Min(100, bytes * 100 / size.Value));
                }
            }
            await using var file = File.OpenRead(downloadedPath);
            byte[] actual = await SHA256.HashDataAsync(file, token);
            if (!Convert.ToHexString(actual).Equals(digest.Value, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("SHA-256 не совпадает. Обновление отменено.");
            progress?.Report(100);
            return downloadedPath;
        }
        catch
        {
            try { Directory.Delete(folder, recursive: true); } catch (Exception cleanupError) when (cleanupError is IOException or UnauthorizedAccessException) { }
            throw;
        }
    }

    public static void LaunchInstaller(string downloadedExe)
    {
        string current = Environment.ProcessPath ?? throw new InvalidOperationException("Не найден путь к программе.");
        if (!File.Exists(current) || !Path.GetExtension(current).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Автообновление работает только из собранного EXE.");
        var start = new ProcessStartInfo(downloadedExe) { UseShellExecute = false };
        start.ArgumentList.Add("--apply-update");
        start.ArgumentList.Add(current);
        start.ArgumentList.Add(Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (Process.Start(start) is null)
            throw new InvalidOperationException("Не удалось запустить установщик.");
    }

    public static string? PreviousVersionPath
    {
        get
        {
            string? current = Environment.ProcessPath;
            if (current is null || !current.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) return null;
            string backup = current + ".previous";
            return File.Exists(backup) && new FileInfo(backup).Length > 0 ? backup : null;
        }
    }

    /// <summary>
    /// The running EXE cannot replace itself. A temporary copy of the current EXE
    /// waits for the app to exit and restores the .previous file in its place.
    /// </summary>
    public static void LaunchRollback()
    {
        string previous = PreviousVersionPath ?? throw new FileNotFoundException(
            "Резервная копия предыдущей версии не найдена.");
        string current = Environment.ProcessPath!;
        string tempFolder = Path.Combine(Path.GetTempPath(), "TextSwap-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        string helper = Path.Combine(tempFolder, "TextSwap.exe");
        try
        {
            File.Copy(current, helper);
            var start = new ProcessStartInfo(helper) { UseShellExecute = false };
            start.ArgumentList.Add("--apply-rollback");
            start.ArgumentList.Add(current);
            start.ArgumentList.Add(Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (Process.Start(start) is null)
                throw new InvalidOperationException("Не удалось запустить откат.");
        }
        catch
        {
            try { Directory.Delete(tempFolder, recursive: true); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            throw;
        }
    }

    public static int ApplyRollback(string[] args)
    {
        try
        {
            if (args.Length != 3 || !int.TryParse(args[2], out int previousPid)) return 2;
            string original = Path.GetFullPath(args[1]);
            if (!original.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || !File.Exists(original))
                throw new FileNotFoundException("Установленный EXE не найден.", original);
            string backup = original + ".previous";
            if (!File.Exists(backup)) throw new FileNotFoundException("Предыдущая версия не найдена.", backup);
            if (previousPid == Environment.ProcessId) throw new InvalidOperationException("Недопустимый PID.");

            try
            {
                using var oldProcess = Process.GetProcessById(previousPid);
                if (!oldProcess.WaitForExit(30_000)) throw new TimeoutException("Программа не закрылась за 30 секунд.");
            }
            catch (ArgumentException) { /* Already exited. */ }

            BackupSwap.RestorePrevious(original);

            // The restored older EXE already understands --cleanup-updater.
            var start = new ProcessStartInfo(original) { UseShellExecute = true };
            start.ArgumentList.Add("--cleanup-updater");
            start.ArgumentList.Add(Environment.ProcessPath!);
            if (Process.Start(start) is null)
                throw new InvalidOperationException("Версия восстановлена, но перезапуск не удался.");
            return 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось выполнить откат. Проверь файлы программы и при необходимости " +
                            "запусти её вручную.\n\n" + ex.Message,
                "Text Swap — откат", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }

    public static int ApplyUpdate(string[] args)
    {
        try
        {
            if (args.Length != 3 || !int.TryParse(args[2], out var previousPid)) return 2;
            string original = Path.GetFullPath(args[1]);
            if (!File.Exists(original)) throw new FileNotFoundException("Исходный EXE не найден", original);
            if (previousPid == Environment.ProcessId) throw new InvalidOperationException("Недопустимый PID.");

            try
            {
                using var oldProcess = Process.GetProcessById(previousPid);
                if (!oldProcess.WaitForExit(30_000)) throw new TimeoutException("Программа не закрылась за 30 секунд.");
            }
            catch (ArgumentException) { /* Already exited. */ }

            string stage = original + ".new";
            string backup = original + ".previous";
            File.Copy(Environment.ProcessPath!, stage, overwrite: true);
            try
            {
                File.Replace(stage, original, backup, ignoreMetadataErrors: true);
            }
            finally
            {
                if (File.Exists(stage)) File.Delete(stage);
            }
            var start = new ProcessStartInfo(original) { UseShellExecute = true };
            start.ArgumentList.Add("--cleanup-updater");
            start.ArgumentList.Add(Environment.ProcessPath!);
            Process.Start(start);
            return 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось установить обновление. Старая версия сохранена, если замена не произошла.\n\n" +
                            ex.Message, "Text Swap — обновление", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 1;
        }
    }

    public static async Task CleanupUpdaterAsync(string updaterPath)
    {
        // The updater cannot delete its own EXE while it is running on Windows.
        for (int i = 0; i < 30; i++)
        {
            await Task.Delay(500);
            try
            {
                if (File.Exists(updaterPath)) File.Delete(updaterPath);
                var parent = Path.GetDirectoryName(updaterPath);
                if (parent is not null && Path.GetFileName(parent).StartsWith("TextSwap-update-", StringComparison.Ordinal))
                    Directory.Delete(parent, recursive: false);
                return;
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { return; }
        }
    }

}
