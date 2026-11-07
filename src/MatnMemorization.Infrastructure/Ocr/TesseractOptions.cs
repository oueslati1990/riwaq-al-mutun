namespace MatnMemorization.Infrastructure.Ocr;

public sealed class TesseractOptions
{
    public string TessDataPath { get; set; } = "tessdata";
    public string Language { get; set; } = "ara";
}
