using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.Contracts.Ocr;

public interface IOcrService
{
    Task<OcrResult> RecognizeAsync(byte[] imageBytes, CancellationToken ct = default);
}
