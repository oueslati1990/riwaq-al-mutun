namespace MatnMemorization.Application.Contracts.Ocr;

public interface IOcrService
{
    Task<string> RecognizeAsync(byte[] imageBytes, CancellationToken ct = default);
}
