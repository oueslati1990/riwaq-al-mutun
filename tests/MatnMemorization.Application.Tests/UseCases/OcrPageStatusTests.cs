using MatnMemorization.Application.Models;
using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Import;

namespace MatnMemorization.Application.Tests.UseCases;

public class OcrPageStatusTests
{
    [Fact]
    public void OcrPageResult_From_StartsNeedsReview()
    {
        var r = OcrPageResult.From(1, "نص");
        Assert.Equal(OcrPageStatus.NeedsReview, r.Status);
    }

    [Fact]
    public void OcrPageResult_IsApproved_SetsStatusApproved()
    {
        var r = OcrPageResult.From(1, "نص");
        r.IsApproved = true;
        Assert.Equal(OcrPageStatus.Approved, r.Status);
    }

    [Fact]
    public void OcrPageResult_IsApprovedFalse_SetsNeedsReview()
    {
        var r = OcrPageResult.From(1, "نص");
        r.IsApproved = true;
        r.IsApproved = false;
        Assert.Equal(OcrPageStatus.NeedsReview, r.Status);
    }

    [Fact]
    public void OcrPageResult_ConfidenceAndImagePreserved()
    {
        byte[] img = [1, 2, 3];
        var r = OcrPageResult.From(1, "نص", 0.87f, img);
        Assert.Equal(0.87f, r.Confidence);
        Assert.Equal(img, r.PageImageBytes);
    }

    [Fact]
    public async Task ImportHandler_UsesOnlyApprovedPages()
    {
        var repo = new FakeMatnRepository();
        var handler = new ImportMatnFromOcrHandler(repo);

        var pages = new List<OcrPageResult>
        {
            OcrPageResult.From(1, "صفحة واحدة"),
            OcrPageResult.From(2, "صفحة ثانية"),
        };
        // only approve page 1
        pages[0].IsApproved = true;

        var approved = pages.Where(p => p.IsApproved).ToList();
        await handler.HandleAsync(new ImportMatnFromOcrCommand("متن", null, approved));

        Assert.Single(repo.Stored[0].Sections);
    }
}
