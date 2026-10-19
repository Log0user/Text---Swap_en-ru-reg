using System.Diagnostics;

namespace TextSwap.Core;

/// <summary>Reads the actual Windows EXE version resource, not the filename.</summary>
public sealed record LocalBackupInfo(string Path, Version Version, long Bytes);

public static class LocalBackup
{
    public static string GetPath(string installedExe) => installedExe + ".previous";

    public static LocalBackupInfo? Inspect(string installedExe)
    {
        string file = GetPath(installedExe);
        if (!File.Exists(file)) return null;
        var length = new FileInfo(file).Length;
        if (length == 0) return null;
        // No metadata means no safe one-click switch. The user may still delete the file.
        var details = FileVersionInfo.GetVersionInfo(file);
        if (details.FileMajorPart < 1) return null;
        var version = new Version(details.FileMajorPart, details.FileMinorPart,
            details.FileBuildPart, details.FilePrivatePart);
        return new LocalBackupInfo(file, version, length);
    }

    public static string SwitchLabel(Version current, Version backup) => backup.CompareTo(current) switch
    {
        > 0 => $"Вернуться к новой ({backup.ToString(3)})",
        < 0 => $"Откатить до ({backup.ToString(3)})",
        _ => $"Переключить на копию ({backup.ToString(3)})"
    };

    /// <summary>Never touches cache folders or settings. Only this application's adjacent backup.</summary>
    public static void Delete(string installedExe)
    {
        string path = GetPath(installedExe);
        if (!File.Exists(path)) return;
        File.Delete(path);
    }
}
