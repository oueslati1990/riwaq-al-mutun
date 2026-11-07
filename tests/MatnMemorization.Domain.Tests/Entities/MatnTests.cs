using MatnMemorization.Domain.Entities;
using MatnMemorization.Domain.Enums;

namespace MatnMemorization.Domain.Tests.Entities;

public class MatnTests
{
    [Fact]
    public void Create_SetsDefaultsCorrectly()
    {
        var matn = Matn.Create("الأجرومية", "ابن آجروم");

        Assert.NotEqual(Guid.Empty, matn.Id);
        Assert.Equal("الأجرومية", matn.Title);
        Assert.Equal("ابن آجروم", matn.Author);
        Assert.Equal(MatnStatus.Draft, matn.Status);
        Assert.False(matn.IsDeleted);
        Assert.Empty(matn.Sections);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_Throws_WhenTitleIsEmpty(string title)
    {
        Assert.Throws<ArgumentException>(() => Matn.Create(title));
    }

    [Fact]
    public void Create_Throws_WhenTitleExceeds200Chars()
    {
        Assert.Throws<ArgumentException>(() => Matn.Create(new string('أ', 201)));
    }

    [Fact]
    public void SoftDelete_SetsIsDeletedAndArchivesStatus()
    {
        var matn = Matn.Create("متن");
        matn.SoftDelete();

        Assert.True(matn.IsDeleted);
        Assert.Equal(MatnStatus.Archived, matn.Status);
    }

    [Fact]
    public void AddSection_AppendsWithIncrementalOrder()
    {
        var matn = Matn.Create("متن");
        var s1 = matn.AddSection("الباب الأول");
        var s2 = matn.AddSection("الباب الثاني");

        Assert.Equal(2, matn.Sections.Count);
        Assert.Equal(1, s1.Order);
        Assert.Equal(2, s2.Order);
    }
}
