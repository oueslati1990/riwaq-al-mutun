using MatnMemorization.Application.Models;
using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Import;

namespace MatnMemorization.Application.Tests.UseCases;

public class ImportMatnFromOcrTests
{

    [Fact]
    public async Task HandleAsync_CreatesMathWithSectionsAndUnits()
    {
        var repo = new FakeMatnRepository();
        var handler = new ImportMatnFromOcrHandler(repo);

        var pages = new List<OcrPageResult>
        {
            OcrPageResult.From(1, "بِسْمِ اللَّهِ\nالرَّحْمَٰنِ الرَّحِيمِ"),
            OcrPageResult.From(2, "الحمد لله\nرب العالمين")
        };
        pages.ForEach(p => p.IsApproved = true);

        var cmd = new ImportMatnFromOcrCommand("متن الاستيراد", "كاتب", pages);
        var id = await handler.HandleAsync(cmd);

        Assert.Single(repo.Stored);
        var matn = repo.Stored[0];
        Assert.Equal("متن الاستيراد", matn.Title);
        Assert.Equal(2, matn.Sections.Count);
        Assert.Equal(2, matn.Sections[0].TextUnits.Count);
        Assert.Equal(2, matn.Sections[1].TextUnits.Count);
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task HandleAsync_SkipsBlankPages()
    {
        var repo = new FakeMatnRepository();
        var handler = new ImportMatnFromOcrHandler(repo);

        var pages = new List<OcrPageResult>
        {
            OcrPageResult.From(1, "متن حقيقي"),
            OcrPageResult.From(2, "   \n  ")  // blank page
        };
        pages.ForEach(p => p.IsApproved = true);

        await handler.HandleAsync(new ImportMatnFromOcrCommand("متن", null, pages));

        Assert.Single(repo.Stored[0].Sections);
    }

    [Fact]
    public async Task HandleAsync_ThrowsWhenTitleEmpty()
    {
        var handler = new ImportMatnFromOcrHandler(new FakeMatnRepository());
        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(new ImportMatnFromOcrCommand("", null, [])));
    }

    [Fact]
    public void OcrPageResult_EditedTextDefaultsToRawText()
    {
        var r = OcrPageResult.From(1, "نص التعرف");
        Assert.Equal("نص التعرف", r.EditedText);
    }

    [Fact]
    public void OcrPageResult_EditedTextCanBeOverridden()
    {
        var r = OcrPageResult.From(1, "نص التعرف");
        r.EditedText = "نص محرر";
        Assert.Equal("نص محرر", r.EditedText);
        Assert.Equal("نص التعرف", r.RawText);
    }
}
