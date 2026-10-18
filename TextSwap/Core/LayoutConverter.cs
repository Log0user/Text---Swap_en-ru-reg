using System.Text;

namespace TextSwap.Core;

/// <summary>
/// Standard Windows RU/US keyboard layout mapping. Not a translation service.
/// Direction is explicit; characters absent from the source layout are left unchanged.
/// </summary>
public static class LayoutConverter
{
    private static readonly IReadOnlyDictionary<char, char> EnglishToRussianMap;
    private static readonly IReadOnlyDictionary<char, char> RussianToEnglishMap;

    static LayoutConverter()
    {
        var enToRu = new Dictionary<char, char>();
        var ruToEn = new Dictionary<char, char>();

        AddPairs("`qwertyuiop[]asdfghjkl;'zxcvbnm,./",
                 "ёйцукенгшщзхъфывапролджэячсмитьбю.", enToRu, ruToEn);
        AddPairs("~QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?",
                 "ЁЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,", enToRu, ruToEn);
        // Shift+digit differs between US and Russian Windows layouts.
        AddPairs("@#$^&", "\"№;:?", enToRu, ruToEn);
        EnglishToRussianMap = enToRu;
        RussianToEnglishMap = ruToEn;
    }

    private static void AddPairs(string english, string russian,
        Dictionary<char, char> toRussian, Dictionary<char, char> toEnglish)
    {
        if (english.Length != russian.Length)
            throw new InvalidOperationException("Invalid keyboard mapping lengths.");
        for (int i = 0; i < english.Length; i++)
        {
            if (!toRussian.TryAdd(english[i], russian[i]) ||
                !toEnglish.TryAdd(russian[i], english[i]))
                throw new InvalidOperationException($"Duplicate keyboard mapping: {english[i]}/{russian[i]}");
        }
    }

    public static string Convert(string text, ConversionMode mode)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length == 0) return text;

        return mode switch
        {
            ConversionMode.EnglishToRussian => ConvertDirection(text, Direction.EnglishToRussian),
            ConversionMode.RussianToEnglish => ConvertDirection(text, Direction.RussianToEnglish),
            ConversionMode.ByFragment => ConvertFragments(text),
            _ => ConvertDirection(text, LayoutDetector.Detect(text))
        };
    }

    public static string ConvertDirection(string text, Direction direction)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (direction == Direction.None || text.Length == 0) return text;
        var map = direction == Direction.EnglishToRussian ? EnglishToRussianMap : RussianToEnglishMap;
        var result = new StringBuilder(text.Length);
        foreach (char c in text)
            result.Append(map.TryGetValue(c, out var replacement) ? replacement : c);
        return result.ToString();
    }

    /// <summary>
    /// Experimental heuristic: determine the alphabet of each run of letters with
    /// directly attached keyboard punctuation. It cannot identify real English words
    /// such as "Python". Mixed script runs are deliberately left unchanged.
    /// </summary>
    private static string ConvertFragments(string text)
    {
        var result = new StringBuilder(text.Length);
        int i = 0;
        while (i < text.Length)
        {
            // A fragment starts with a letter or with layout punctuation attached to a letter.
            if (!IsAlphabetic(text[i]) && !IsLayoutSymbol(text[i]))
            {
                result.Append(text[i++]);
                continue;
            }
            int start = i;
            while (i < text.Length && (IsAlphabetic(text[i]) || IsLayoutSymbol(text[i]))) i++;
            string segment = text[start..i];
            var direction = LayoutDetector.Detect(segment);
            result.Append(ConvertDirection(segment, direction));
        }
        return result.ToString();
    }

    private static bool IsAlphabetic(char c) => LayoutDetector.IsEnglish(c) || LayoutDetector.IsRussian(c);

    // Symbols are attached to a fragment, not guessed globally when there are no letters.
    private static bool IsLayoutSymbol(char c) => c is '`' or '~' or '[' or ']' or '{' or '}'
        or ';' or ':' or '\'' or '"' or ',' or '.' or '/' or '?' or '<' or '>' or '№'
        or '@' or '#' or '$' or '^' or '&';
}
