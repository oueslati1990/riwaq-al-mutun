using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
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
    [ObservableProperty] private string _newTitle = string.Empty;
    [ObservableProperty] private string _newAuthor = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isImporting;

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
        if (page is null) return;
        page.IsApproved = true;
        page.Flush();
    }

    [RelayCommand]
    private void ApproveAll()
    {
        foreach (var p in Pages)
        {
            p.IsApproved = true;
            p.Flush();
        }
    }

    [RelayCommand]
    private void OpenImportDialog()
    {
        // Flush all edits before opening dialog
        foreach (var p in Pages) p.Flush();

        if (!Pages.Any(p => p.IsApproved))
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
            var approved = Pages.Where(p => p.IsApproved).Select(p => p.Source).ToList();
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
