using System.IO;
using Docnet.Core;
using Docnet.Core.Models;
using MatnMemorization.Application.Contracts.Pdf;
using SkiaSharp;

namespace MatnMemorization.Infrastructure.Pdf;

public sealed class DocnetPdfService : IPdfService
{
    private const int MaxFileSizeBytes = 50 * 1024 * 1024;
    private const int MaxPageCount = 200;

    public int GetPageCount(string filePath)
    {
        ValidateFile(filePath);
        using var reader = DocLib.Instance.GetDocReader(filePath, new PageDimensions(1, 1));
        return reader.GetPageCount();
    }

    public async Task<byte[]> RenderPageAsync(string filePath, int pageNumber, int dpi = 300, CancellationToken ct = default)
    {
        ValidateFile(filePath);
        return await Task.Run(() => RenderPage(filePath, pageNumber, dpi), ct);
    }

    private static byte[] RenderPage(string filePath, int pageNumber, int dpi)
    {
        // A4 at requested DPI: width = 8.27" × dpi, height = 11.69" × dpi
        int targetWidth = (int)(8.27 * dpi);
        int targetHeight = (int)(11.69 * dpi);

        using var reader = DocLib.Instance.GetDocReader(filePath, new PageDimensions(targetWidth, targetHeight));
        using var pageReader = reader.GetPageReader(pageNumber);

        int width = pageReader.GetPageWidth();
        int height = pageReader.GetPageHeight();
        byte[] bgra = pageReader.GetImage();

        // Convert BGRA → PNG for Tesseract
        var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        using var bmp = new SKBitmap(info);
        System.Runtime.InteropServices.Marshal.Copy(bgra, 0, bmp.GetPixels(), bgra.Length);
        using var encoded = bmp.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }

    private static void ValidateFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("الملف غير موجود.", filePath);

        var size = new FileInfo(filePath).Length;
        if (size > MaxFileSizeBytes)
            throw new InvalidOperationException($"حجم الملف يتجاوز الحد الأقصى (50 MB). الحجم الفعلي: {size / 1024 / 1024} MB");
    }
}
