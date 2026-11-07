using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Application.Contracts.Ocr;
using MatnMemorization.Application.Contracts.Pdf;
using MatnMemorization.Application.Contracts.UI;
using MatnMemorization.Application.Models;
using MatnMemorization.Presentation.Wpf.Services;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class ImportViewModel : ViewModelBase
{
    private static readonly string FileFilter =
        "ملفات مدعومة|*.pdf;*.png;*.jpg;*.jpeg;*.tiff;*.tif|PDF|*.pdf|صور|*.png;*.jpg;*.jpeg;*.tiff;*.tif";

    private const long MaxBytes = 50L * 1024 * 1024;
    private const int MaxPages = 200;

    private readonly IFileDialogService _filePicker;
    private readonly IPdfService _pdfService;
    private readonly IOcrService _ocrService;
    private readonly INavigationService _navigation;
    private readonly OcrSessionStore _sessionStore;

    private CancellationTokenSource? _cts;

    [ObservableProperty] private string? _filePath;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private int _progress;
    [ObservableProperty] private string _statusMessage = string.Empty;
    [ObservableProperty] private string? _errorMessage;

    public ImportViewModel(
        IFileDialogService filePicker,
        IPdfService pdfService,
        IOcrService ocrService,
        INavigationService navigation,
        OcrSessionStore sessionStore)
    {
        _filePicker = filePicker;
        _pdfService = pdfService;
        _ocrService = ocrService;
        _navigation = navigation;
        _sessionStore = sessionStore;
    }

    [RelayCommand]
    private void PickFile()
    {
        var path = _filePicker.PickFile(FileFilter, "اختر ملف PDF أو صورة");
        if (path is null) return;

        var info = new FileInfo(path);
        if (info.Length > MaxBytes)
        {
            ErrorMessage = $"حجم الملف يتجاوز 50 MB ({info.Length / 1024 / 1024} MB).";
            return;
        }

        FilePath = path;
        ErrorMessage = null;
        StatusMessage = $"تم اختيار: {Path.GetFileName(path)}";
    }

    [RelayCommand(CanExecute = nameof(CanStartOcr))]
    private async Task StartOcrAsync()
    {
        if (FilePath is null) return;

        _cts = new CancellationTokenSource();
        IsProcessing = true;
        ErrorMessage = null;
        Progress = 0;

        try
        {
            var ext = Path.GetExtension(FilePath).ToLowerInvariant();
            var results = ext == ".pdf"
                ? await ProcessPdfAsync(FilePath, _cts.Token)
                : await ProcessImageAsync(FilePath, _cts.Token);

            _sessionStore.Results = results;
            _navigation.NavigateTo<OcrReviewViewModel>();
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "تم إلغاء العملية.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsProcessing = false;
            _cts?.Dispose();
            _cts = null;
        }
    }

    private bool CanStartOcr() => FilePath is not null && !IsProcessing;

    partial void OnFilePathChanged(string? value) => StartOcrCommand.NotifyCanExecuteChanged();
    partial void OnIsProcessingChanged(bool value) => StartOcrCommand.NotifyCanExecuteChanged();

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    private async Task<IReadOnlyList<OcrPageResult>> ProcessPdfAsync(string path, CancellationToken ct)
    {
        int total = _pdfService.GetPageCount(path);
        if (total > MaxPages)
            throw new InvalidOperationException($"عدد الصفحات ({total}) يتجاوز الحد الأقصى ({MaxPages}).");

        var results = new List<OcrPageResult>(total);
        var prog = new Progress<int>(p =>
        {
            Progress = p;
            StatusMessage = $"جارٍ التعرف على الصفحة {p * total / 100 + 1} من {total}...";
        });

        for (int i = 0; i < total; i++)
        {
            ct.ThrowIfCancellationRequested();
            StatusMessage = $"صفحة {i + 1} من {total}...";
            var png = await _pdfService.RenderPageAsync(path, i, 300, ct);
            var text = await _ocrService.RecognizeAsync(png, ct);
            results.Add(OcrPageResult.From(i + 1, text));
            Progress = (i + 1) * 100 / total;
        }

        return results;
    }

    private async Task<IReadOnlyList<OcrPageResult>> ProcessImageAsync(string path, CancellationToken ct)
    {
        StatusMessage = "جارٍ التعرف على الصورة...";
        var bytes = await File.ReadAllBytesAsync(path, ct);
        var text = await _ocrService.RecognizeAsync(bytes, ct);
        Progress = 100;
        return [OcrPageResult.From(1, text)];
    }
}
