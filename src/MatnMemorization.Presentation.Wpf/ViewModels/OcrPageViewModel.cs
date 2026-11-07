using CommunityToolkit.Mvvm.ComponentModel;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class OcrPageViewModel : ObservableObject
{
    public OcrPageResult Source { get; }

    [ObservableProperty] private string _editedText;
    [ObservableProperty] private bool _isApproved;

    public int PageNumber => Source.PageNumber;

    public OcrPageViewModel(OcrPageResult source)
    {
        Source = source;
        _editedText = source.RawText;
        _isApproved = false;
    }

    public void Flush()
    {
        Source.EditedText = EditedText;
        Source.IsApproved = IsApproved;
    }
}
