using MatnMemorization.Application.Models;

namespace MatnMemorization.Presentation.Wpf.Services;

public sealed class OcrSessionStore
{
    public IReadOnlyList<OcrPageResult> Results { get; set; } = [];
}
