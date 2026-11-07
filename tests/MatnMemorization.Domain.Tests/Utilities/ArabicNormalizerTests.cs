using MatnMemorization.Domain.Utilities;

namespace MatnMemorization.Domain.Tests.Utilities;

public class ArabicNormalizerTests
{
    [Theory]
    [InlineData("تَحْفِيظُ القُرآن", "تحفيظ القران")]   // strip diacritics + alef madda
    [InlineData("أحمد", "احمد")]                          // hamza above → bare alef
    [InlineData("إبراهيم", "ابراهيم")]                   // hamza below → bare alef
    [InlineData("آدم", "ادم")]                             // alef madda → bare alef
    [InlineData("فاطمة", "فاطمه")]                        // teh marbuta → heh
    [InlineData("موسى", "موسي")]                           // alef maqsura → yeh
    [InlineData("مُحَمَّدٌ", "محمد")]                   // diacritics stripped
    public void ToSearchIndex_NormalizesCorrectly(string input, string expected)
    {
        var result = ArabicNormalizer.ToSearchIndex(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToSearchIndex_ReturnsEmpty_ForNullOrWhitespace()
    {
        Assert.Equal(string.Empty, ArabicNormalizer.ToSearchIndex(""));
        Assert.Equal(string.Empty, ArabicNormalizer.ToSearchIndex("   "));
    }

    [Fact]
    public void ToSearchIndex_PreservesNonArabicChars()
    {
        var result = ArabicNormalizer.ToSearchIndex("سورة 2");
        Assert.Contains("2", result);
    }
}
