using MatnMemorization.Application.Contracts.Ocr;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tesseract;

namespace MatnMemorization.Infrastructure.Ocr;

public sealed class TesseractOcrService : IOcrService, IDisposable
{
    private readonly TesseractEngine _engine;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly ILogger<TesseractOcrService> _logger;
    private bool _disposed;

    public TesseractOcrService(IOptions<TesseractOptions> options, ILogger<TesseractOcrService> logger)
    {
        _logger = logger;
        var opts = options.Value;
        var dataPath = Path.IsPathRooted(opts.TessDataPath)
            ? opts.TessDataPath
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, opts.TessDataPath);

        if (!Directory.Exists(dataPath))
            throw new InvalidOperationException(
                $"مجلد tessdata غير موجود: {dataPath}\nضع ملف ara.traineddata في {dataPath}");

        _engine = new TesseractEngine(dataPath, opts.Language, EngineMode.Default);
        // Arabic is RTL — tell Tesseract
        _engine.DefaultPageSegMode = PageSegMode.Auto;
    }

    public async Task<string> RecognizeAsync(byte[] imageBytes, CancellationToken ct = default)
    {
        await _lock.WaitAsync(ct);
        try
        {
            return await Task.Run(() =>
            {
                using var pix = Pix.LoadFromMemory(imageBytes);
                using var page = _engine.Process(pix);
                var text = page.GetText();
                _logger.LogDebug("OCR confidence: {Confidence:F1}%", page.GetMeanConfidence() * 100);
                return text ?? string.Empty;
            }, ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _engine.Dispose();
        _lock.Dispose();
    }
}
