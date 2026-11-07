using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Domain.Tests.Entities;

public class TextUnitTests
{
    [Fact]
    public void Create_SetsSearchIndexFromContent()
    {
        var unit = TextUnit.Create(Guid.NewGuid(), "بِسْمِ اللَّهِ الرَّحْمَٰنِ الرَّحِيمِ", 1);

        Assert.NotEmpty(unit.SearchIndex);
        Assert.DoesNotContain("ً", unit.SearchIndex); // no fathatan
        Assert.Equal("بِسْمِ اللَّهِ الرَّحْمَٰنِ الرَّحِيمِ", unit.Content); // original preserved
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Throws_WhenContentEmpty(string content)
    {
        Assert.Throws<ArgumentException>(() => TextUnit.Create(Guid.NewGuid(), content, 1));
    }

    [Fact]
    public void Create_Throws_WhenContentExceeds2000Chars()
    {
        Assert.Throws<ArgumentException>(() => TextUnit.Create(Guid.NewGuid(), new string('أ', 2001), 1));
    }

    [Fact]
    public void UpdateContent_UpdatesSearchIndexToo()
    {
        var unit = TextUnit.Create(Guid.NewGuid(), "أَحمد", 1);
        var oldIndex = unit.SearchIndex;

        unit.UpdateContent("إبراهيم");

        Assert.NotEqual(oldIndex, unit.SearchIndex);
        Assert.Equal("إبراهيم", unit.Content);
    }
}
