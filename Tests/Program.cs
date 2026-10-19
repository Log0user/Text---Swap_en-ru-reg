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
Console.WriteLine($"PASSED: {passed} tests");
