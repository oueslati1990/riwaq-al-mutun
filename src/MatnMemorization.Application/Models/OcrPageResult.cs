namespace MatnMemorization.Application.Models;

public sealed class OcrPageResult
{
    public int PageNumber { get; init; }
    public string RawText { get; init; } = string.Empty;
    public string EditedText { get; set; } = string.Empty;
    public float Confidence { get; set; }
    public OcrPageStatus Status { get; set; } = OcrPageStatus.Pending;
    public byte[]? PageImageBytes { get; set; }

    // IsApproved is derived from Status for backward compat
    public bool IsApproved
    {
        get => Status == OcrPageStatus.Approved;
        set => Status = value ? OcrPageStatus.Approved : OcrPageStatus.NeedsReview;
    }

    public static OcrPageResult From(int pageNumber, string rawText, float confidence = 0f, byte[]? imageBytes = null) =>
        new()
        {
            PageNumber = pageNumber,
            RawText = rawText,
            EditedText = rawText,
            Confidence = confidence,
            Status = OcrPageStatus.NeedsReview,
            PageImageBytes = imageBytes
        };
}
