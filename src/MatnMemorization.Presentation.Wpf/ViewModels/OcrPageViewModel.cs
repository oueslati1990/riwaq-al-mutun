using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class OcrPageViewModel : ObservableObject
{
    public OcrPageResult Source { get; }

    [ObservableProperty] private string _editedText;
    [ObservableProperty] private OcrPageStatus _status;

    public int PageNumber => Source.PageNumber;
    public int ConfidencePercent => (int)(Source.Confidence * 100);
    public bool IsLowConfidence => Source.Confidence < 0.60f && Source.Confidence > 0f;

    private BitmapSource? _pageImage;
    public BitmapSource? PageImage => _pageImage ??= LoadImage();

    public OcrPageViewModel(OcrPageResult source)
    {
        Source = source;
        _editedText = source.RawText;
        _status = source.Status;
    }

    private BitmapSource? LoadImage()
    {
        if (Source.PageImageBytes is not { Length: > 0 } bytes) return null;
        var bmp = new BitmapImage();
        using var ms = new MemoryStream(bytes);
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    public void Flush()
    {
        Source.EditedText = EditedText;
        Source.Status = Status;
        Source.IsApproved = Status == OcrPageStatus.Approved;
    }

    public bool TryApprove()
    {
        Status = OcrPageStatus.Approved;
        Flush();
        return true;
    }

    // Returns true if edit is allowed; false means confirmation needed
    public bool RequestEdit()
    {
        if (Status != OcrPageStatus.Approved) return true;
        return false; // caller must show confirmation dialog
    }

    public void ForceReEdit()
    {
        Status = OcrPageStatus.NeedsReview;
        Flush();
    }
}
