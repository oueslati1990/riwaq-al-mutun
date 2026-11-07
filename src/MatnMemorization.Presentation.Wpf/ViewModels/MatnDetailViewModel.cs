using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Application.UseCases.Matns;
using MatnMemorization.Application.UseCases.Sections;
using MatnMemorization.Application.UseCases.TextUnits;
using MatnMemorization.Domain.Entities;
using MatnMemorization.Presentation.Wpf.Services;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class MatnDetailViewModel : ViewModelBase
{
    private readonly GetMatnDetailHandler _getDetail;
    private readonly AddSectionHandler _addSection;
    private readonly AddTextUnitHandler _addUnit;
    private readonly MatnSelectionService _selection;
    private readonly INavigationService _navigation;

    [ObservableProperty]
    private Matn? _matn;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    // Add section dialog
    [ObservableProperty]
    private bool _isAddSectionOpen;

    [ObservableProperty]
    private string _newSectionTitle = string.Empty;

    // Add text unit dialog
    [ObservableProperty]
    private bool _isAddUnitOpen;

    [ObservableProperty]
    private string _newUnitContent = string.Empty;

    [ObservableProperty]
    private MatnSection? _selectedSection;

    public MatnDetailViewModel(
        GetMatnDetailHandler getDetail,
        AddSectionHandler addSection,
        AddTextUnitHandler addUnit,
        MatnSelectionService selection,
        INavigationService navigation)
    {
        _getDetail = getDetail;
        _addSection = addSection;
        _addUnit = addUnit;
        _selection = selection;
        _navigation = navigation;
    }

    [RelayCommand]
    private void StartSession()
    {
        if (_selection.SelectedMatnId is null) return;
        _navigation.NavigateTo<MemorizationViewModel>();
    }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct = default)
    {
        if (_selection.SelectedMatnId is null) return;
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            Matn = await _getDetail.HandleAsync(new GetMatnDetailQuery(_selection.SelectedMatnId.Value), ct);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void OpenAddSection()
    {
        NewSectionTitle = string.Empty;
        IsAddSectionOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmAddSectionAsync(CancellationToken ct = default)
    {
        if (Matn is null || string.IsNullOrWhiteSpace(NewSectionTitle)) return;
        try
        {
            await _addSection.HandleAsync(new AddSectionCommand(Matn.Id, NewSectionTitle), ct);
            IsAddSectionOpen = false;
            await LoadAsync(ct);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void CancelAddSection() => IsAddSectionOpen = false;

    [RelayCommand]
    private void OpenAddUnit(MatnSection section)
    {
        SelectedSection = section;
        NewUnitContent = string.Empty;
        IsAddUnitOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmAddUnitAsync(CancellationToken ct = default)
    {
        if (SelectedSection is null || string.IsNullOrWhiteSpace(NewUnitContent)) return;
        try
        {
            await _addUnit.HandleAsync(new AddTextUnitCommand(SelectedSection.Id, NewUnitContent), ct);
            IsAddUnitOpen = false;
            await LoadAsync(ct);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void CancelAddUnit() => IsAddUnitOpen = false;
}
