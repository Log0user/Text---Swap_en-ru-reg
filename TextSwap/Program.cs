using TextSwap.Core;
using TextSwap.Updates;

namespace TextSwap;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--apply-rollback")
        {
            Environment.ExitCode = UpdateService.ApplyRollback(args);
            return;
        }

        if (args.Length > 0 && args[0] == "--apply-update")
        {
            Environment.ExitCode = UpdateService.ApplyUpdate(args);
            return;
        }

        ApplicationConfiguration.Initialize();
        using var mutex = new Mutex(initiallyOwned: true, "Local\\TextSwap-OnlyOneInstance", out bool isFirst);
        if (!isFirst)
        {
            MessageBox.Show("Text Swap уже работает. Найди её значок в системном трее.",
                "Text Swap", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (args.Length == 2 && args[0] == "--cleanup-updater")
            _ = Task.Run(() => UpdateService.CleanupUpdaterAsync(args[1]));

        var settings = AppSettings.Load();
        Application.Run(new MainForm(settings));
        GC.KeepAlive(mutex);
    }
}
