using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Presentation.Wpf.Services.Navigation;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigation;

    public NavigationService NavigationService => (NavigationService)_navigation;

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void NavigateToDashboard() => _navigation.NavigateTo<DashboardViewModel>();

    [RelayCommand]
    private void NavigateToLibrary() => _navigation.NavigateTo<LibraryViewModel>();

    [RelayCommand]
    private void NavigateToImport() => _navigation.NavigateTo<ImportViewModel>();

    [RelayCommand]
    private void NavigateToStatistics() => _navigation.NavigateTo<StatisticsViewModel>();

    [RelayCommand]
    private void NavigateToSettings() => _navigation.NavigateTo<SettingsViewModel>();
}
