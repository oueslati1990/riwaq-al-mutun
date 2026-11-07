namespace MatnMemorization.Application.Models;

public sealed class OcrPageResult
{
    public int PageNumber { get; init; }
    public string RawText { get; init; } = string.Empty;
    public string EditedText { get; set; } = string.Empty;
    public bool IsApproved { get; set; }

    public static OcrPageResult From(int pageNumber, string rawText) =>
        new() { PageNumber = pageNumber, RawText = rawText, EditedText = rawText };
}
