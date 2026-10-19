using TextSwap.Core;

int passed = 0;
void Assert(string expected, string actual, string reason)
{
    if (actual != expected)
        throw new Exception($"{reason}: expected <{expected}>, got <{actual}>");
    Console.WriteLine("OK " + reason);
    passed++;
}

Assert("привет", LayoutConverter.Convert("ghbdtn", ConversionMode.Auto), "EN to RU auto");
Assert("ghbdtn", LayoutConverter.Convert("привет", ConversionMode.Auto), "RU to EN auto");
Assert("пойдет кошка мимо юга",
    LayoutConverter.Convert("gjqltn rjirf vbvj .uf", ConversionMode.Auto), "period is Ю key");
Assert("привет.", LayoutConverter.Convert("ghbdtn/", ConversionMode.Auto), "slash is dot key");
Assert("Привет", LayoutConverter.Convert("Ghbdtn", ConversionMode.EnglishToRussian), "case preserved");
Assert("Привет\nмир\t!", LayoutConverter.Convert("Ghbdtn\nvbh\t!", ConversionMode.EnglishToRussian), "line breaks and tabs preserved");
Assert("123/.", LayoutConverter.Convert("123/.", ConversionMode.Auto), "no-letters safe default");
Assert("приветб мир", LayoutConverter.Convert("ghbdtn, мир", ConversionMode.EnglishToRussian), "strict keyboard mapping and opposite alphabet remains");
Assert("юга", LayoutConverter.Convert(".uf", ConversionMode.ByFragment), "fragment with leading punctuation");
Assert("Привет мир!", LayoutConverter.Convert("Привет мир!", ConversionMode.EnglishToRussian), "same alphabet is preserved");
Assert("ghbdtn", LayoutConverter.ConvertDirection("привет", Direction.RussianToEnglish), "explicit reverse");

string englishKeys = "`qwertyuiop[]asdfghjkl;'zxcvbnm,./~QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?@#$^&";
var inverted = LayoutConverter.ConvertDirection(
    LayoutConverter.ConvertDirection(englishKeys, Direction.EnglishToRussian),
    Direction.RussianToEnglish);
Assert(englishKeys, inverted, "mapping bijection for supported symbols");

// The updater's rollback operation only touches EXE files, not saved settings.
var tempDir = Path.Combine(Path.GetTempPath(), "TextSwap-test-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(tempDir);
try
{
    string exe = Path.Combine(tempDir, "TextSwap.exe");
    File.WriteAllText(exe, "version 2.0.1");
    File.WriteAllText(exe + ".previous", "version 2.0.0");
    BackupSwap.RestorePrevious(exe);
    Assert("version 2.0.0", File.ReadAllText(exe), "rollback restores previous EXE");
    Assert("version 2.0.1", File.ReadAllText(exe + ".previous"), "rollback saves displaced EXE");
    Assert("False", File.Exists(exe + ".rollback-stage").ToString(), "rollback removes staging file");
}
finally
{
    Directory.Delete(tempDir, recursive: true);
}
Assert("System", AppTheme.System.ToString(), "system theme is available");
Assert("Light", AppTheme.Light.ToString(), "light theme is available");
Assert("Dark", AppTheme.Dark.ToString(), "dark theme is available");

Assert("Откатить до (2.0.1)", LocalBackup.SwitchLabel(new Version(2,0,2), new Version(2,0,1)), "older cached exe label");
Assert("Вернуться к новой (2.0.2)", LocalBackup.SwitchLabel(new Version(2,0,1), new Version(2,0,2)), "newer cached exe label");
Assert("Переключить на копию (2.0.2)", LocalBackup.SwitchLabel(new Version(2,0,2), new Version(2,0,2)), "same version cache label");
var cacheDir = Path.Combine(Path.GetTempPath(), "TextSwap-cache-test-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(cacheDir);
try
{
    var running = Path.Combine(cacheDir, "TextSwap.exe");
    File.WriteAllText(running, "2.0.2 test");
    File.WriteAllText(LocalBackup.GetPath(running), "2.0.1 test");
    LocalBackup.Delete(running);
    Assert("True", File.Exists(running).ToString(), "deleting cache leaves current executable intact");
    Assert("False", File.Exists(LocalBackup.GetPath(running)).ToString(), "delete only removes .previous");
}
finally { Directory.Delete(cacheDir, recursive: true); }
Console.WriteLine($"PASSED: {passed} tests");
