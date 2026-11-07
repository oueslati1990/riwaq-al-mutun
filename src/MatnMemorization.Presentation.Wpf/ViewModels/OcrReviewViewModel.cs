using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Application.Models;
using MatnMemorization.Application.UseCases.Import;
using MatnMemorization.Presentation.Wpf.Services;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class OcrReviewViewModel : ViewModelBase
{
    private readonly ImportMatnFromOcrHandler _importHandler;
    private readonly MatnSelectionService _selection;
    private readonly INavigationService _navigation;
    private readonly OcrSessionStore _sessionStore;

    public ObservableCollection<OcrPageViewModel> Pages { get; } = [];

    [ObservableProperty] private OcrPageViewModel? _selectedPage;
    [ObservableProperty] private bool _isImportDialogOpen;
    [ObservableProperty] private bool _isReEditConfirmOpen;
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newAuthor = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isImporting;

    private OcrPageViewModel? _pageRequestingReEdit;

    public OcrReviewViewModel(
        ImportMatnFromOcrHandler importHandler,
        MatnSelectionService selection,
        INavigationService navigation,
        OcrSessionStore sessionStore)
    {
        _importHandler = importHandler;
        _selection = selection;
        _navigation = navigation;
        _sessionStore = sessionStore;
    }

    [RelayCommand]
    private void Load()
    {
        Pages.Clear();
        foreach (var r in _sessionStore.Results)
            Pages.Add(new OcrPageViewModel(r));
        SelectedPage = Pages.FirstOrDefault();
    }

    [RelayCommand]
    private void ApprovePage(OcrPageViewModel? page)
    {
        (page ?? SelectedPage)?.TryApprove();
    }

    [RelayCommand]
    private void ApproveAll()
    {
        foreach (var p in Pages) p.TryApprove();
    }

    // Called when user starts typing in the text box for the selected page
    [RelayCommand]
    private void RequestEditPage(OcrPageViewModel? page)
    {
        var target = page ?? SelectedPage;
        if (target is null) return;

        if (!target.RequestEdit())
        {
            _pageRequestingReEdit = target;
            IsReEditConfirmOpen = true;
        }
    }

    [RelayCommand]
    private void ConfirmReEdit()
    {
        _pageRequestingReEdit?.ForceReEdit();
        _pageRequestingReEdit = null;
        IsReEditConfirmOpen = false;
    }

    [RelayCommand]
    private void CancelReEdit()
    {
        _pageRequestingReEdit = null;
        IsReEditConfirmOpen = false;
    }

    [RelayCommand]
    private void OpenImportDialog()
    {
        foreach (var p in Pages) p.Flush();

        if (!Pages.Any(p => p.Status == OcrPageStatus.Approved))
        {
            ErrorMessage = "اعتمد صفحة واحدة على الأقل قبل الحفظ.";
            return;
        }

        ErrorMessage = null;
        NewTitle = string.Empty;
        NewAuthor = string.Empty;
        IsImportDialogOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmImportAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(NewTitle))
        {
            ErrorMessage = "العنوان مطلوب.";
            return;
        }

        IsImporting = true;
        try
        {
            var approved = Pages
                .Where(p => p.Status == OcrPageStatus.Approved)
                .Select(p => p.Source)
                .ToList();

            var cmd = new ImportMatnFromOcrCommand(NewTitle, NewAuthor, approved);
            var id = await _importHandler.HandleAsync(cmd, ct);

            _selection.SelectedMatnId = id;
            IsImportDialogOpen = false;
            _navigation.NavigateTo<MatnDetailViewModel>();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsImporting = false;
        }
    }

    [RelayCommand]
    private void CancelImport() => IsImportDialogOpen = false;
}
