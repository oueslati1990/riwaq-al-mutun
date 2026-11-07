namespace MatnMemorization.Application.Contracts.Pdf;

public interface IPdfService
{
    int GetPageCount(string filePath);
    Task<byte[]> RenderPageAsync(string filePath, int pageNumber, int dpi = 300, CancellationToken ct = default);
}
