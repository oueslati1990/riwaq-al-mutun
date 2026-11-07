using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Application.UseCases.Matns;
using MatnMemorization.Domain.Entities;
using MatnMemorization.Presentation.Wpf.Services;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class LibraryViewModel : ViewModelBase
{
    private readonly GetMatnListHandler _getList;
    private readonly CreateMatnHandler _create;
    private readonly SoftDeleteMatnHandler _softDelete;
    private readonly INavigationService _navigation;
    private readonly MatnSelectionService _selection;

    public ObservableCollection<Matn> Matns { get; } = [];

    [ObservableProperty]
    private string _searchTerm = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    // Create dialog fields
    [ObservableProperty]
    private bool _isCreateDialogOpen;

    [ObservableProperty]
    private string _newTitle = string.Empty;

    [ObservableProperty]
    private string _newAuthor = string.Empty;

    [ObservableProperty]
    private string _newCategory = string.Empty;

    public LibraryViewModel(
        GetMatnListHandler getList,
        CreateMatnHandler create,
        SoftDeleteMatnHandler softDelete,
        INavigationService navigation,
        MatnSelectionService selection)
    {
        _getList = getList;
        _create = create;
        _softDelete = softDelete;
        _navigation = navigation;
        _selection = selection;
    }

    partial void OnSearchTermChanged(string value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var items = await _getList.HandleAsync(new GetMatnListQuery(SearchTerm), ct);
            Matns.Clear();
            foreach (var m in items)
                Matns.Add(m);
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
    private void OpenCreateDialog()
    {
        NewTitle = string.Empty;
        NewAuthor = string.Empty;
        NewCategory = string.Empty;
        IsCreateDialogOpen = true;
    }

    [RelayCommand]
    private async Task ConfirmCreateAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return;
        try
        {
            await _create.HandleAsync(
                new CreateMatnCommand(NewTitle, NewAuthor.NullIfEmpty(), NewCategory.NullIfEmpty(), null), ct);
            IsCreateDialogOpen = false;
            await LoadAsync(ct);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void CancelCreate() => IsCreateDialogOpen = false;

    [RelayCommand]
    private void OpenDetail(Matn matn)
    {
        _selection.SelectedMatnId = matn.Id;
        _navigation.NavigateTo<MatnDetailViewModel>();
    }

    [RelayCommand]
    private async Task DeleteMatnAsync(Matn matn, CancellationToken ct = default)
    {
        try
        {
            await _softDelete.HandleAsync(new SoftDeleteMatnCommand(matn.Id), ct);
            Matns.Remove(matn);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

internal static class StringExtensions
{
    internal static string? NullIfEmpty(this string s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}
