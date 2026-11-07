using System.Text;
using System.Text.RegularExpressions;

namespace MatnMemorization.Domain.Utilities;

/// <summary>
/// Produces a diacritic-stripped, letter-normalized index string for search.
/// The result is NEVER displayed to the user — only stored for matching.
/// Original text with diacritics is always preserved separately.
/// </summary>
public static class ArabicNormalizer
{
    // Diacritics (tashkeel) + tatweel
    private static readonly Regex StripPattern = new(
        @"[ؐ-ًؚ-ٰٟۖ-ۜ۟-۪ۤۧۨ-ۭـ]",
        RegexOptions.Compiled);

    public static string ToSearchIndex(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var stripped = StripPattern.Replace(text, string.Empty);
        var sb = new StringBuilder(stripped.Length);

        foreach (var ch in stripped)
        {
            sb.Append(ch switch
            {
                // Alef variants → bare alef
                'آ' or 'أ' or 'إ' or 'ٱ' => 'ا',
                // Teh marbuta → heh
                'ة' => 'ه',
                // Alef maqsura → yeh
                'ى' => 'ي',
                _ => ch
            });
        }

        return sb.ToString();
    }
}
