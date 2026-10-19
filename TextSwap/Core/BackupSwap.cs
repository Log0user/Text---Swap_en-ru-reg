namespace TextSwap.Core;

/// <summary>Exchanges the installed EXE and its single .previous backup.</summary>
public static class BackupSwap
{
    public static void RestorePrevious(string installedExe)
    {
        if (!File.Exists(installedExe))
            throw new FileNotFoundException("Установленная программа не найдена.", installedExe);

        string backup = installedExe + ".previous";
        if (!File.Exists(backup) || new FileInfo(backup).Length == 0)
            throw new FileNotFoundException("Резервная копия не найдена.", backup);

        string staged = installedExe + ".rollback-stage";
        // The original backup stays untouched until File.Replace succeeds.
        File.Copy(backup, staged, overwrite: true);
        try
        {
            // Current gets the previous version; .previous gets the displaced current.
            File.Replace(staged, installedExe, backup, ignoreMetadataErrors: true);
        }
        finally
        {
            if (File.Exists(staged)) File.Delete(staged);
        }
    }
}
