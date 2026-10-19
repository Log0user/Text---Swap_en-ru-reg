using System.Text.Json;

namespace TextSwap.Core;

public sealed class AppSettings
{
    public ConversionMode Mode { get; set; } = ConversionMode.Auto;
    public AppTheme Theme { get; set; } = AppTheme.System;
    public bool AutoCopyResult { get; set; } = false;
    public bool StartMinimized { get; set; } = false;
    public bool MinimizeToTray { get; set; } = true;
    public bool CloseToTray { get; set; } = true;
    public bool EnableGlobalHotkeys { get; set; } = true;

    public static string SettingsDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TextSwap");
    public static string SettingsPath => Path.Combine(SettingsDirectory, "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (!File.Exists(SettingsPath)) return new AppSettings();
            var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new();
            if (!Enum.IsDefined(settings.Mode)) settings.Mode = ConversionMode.Auto;
            if (!Enum.IsDefined(settings.Theme)) settings.Theme = AppTheme.System;
            return settings;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            // Malformed settings never prevent the application from starting.
            return new AppSettings();
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(SettingsDirectory);
        string temp = SettingsPath + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        File.Move(temp, SettingsPath, true);
    }
}
