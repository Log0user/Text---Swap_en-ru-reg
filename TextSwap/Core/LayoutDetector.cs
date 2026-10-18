namespace TextSwap.Core;

/// <summary>Detects the dominant alphabet; it never guesses from punctuation alone.</summary>
public static class LayoutDetector
{
    public static Direction Detect(ReadOnlySpan<char> text)
    {
        int english = 0, russian = 0;
        foreach (char c in text)
        {
            if (IsEnglish(c)) english++;
            else if (IsRussian(c)) russian++;
        }

        if (english == russian) return Direction.None;
        return english > russian ? Direction.EnglishToRussian : Direction.RussianToEnglish;
    }

    public static bool IsEnglish(char c) => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    public static bool IsRussian(char c) => c is >= 'а' and <= 'я' or >= 'А' and <= 'Я' or 'ё' or 'Ё';
}
