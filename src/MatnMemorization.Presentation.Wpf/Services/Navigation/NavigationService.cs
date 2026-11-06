using CommunityToolkit.Mvvm.ComponentModel;
using MatnMemorization.Application.Contracts.Navigation;
using MatnMemorization.Presentation.Wpf.ViewModels;
using Microsoft.Extensions.Logging;

namespace MatnMemorization.Presentation.Wpf.Services.Navigation;

public class NavigationService : ObservableObject, INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;

    private ViewModelBase? _currentViewModel;
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
        => NavigateTo(typeof(TViewModel));

    public void NavigateTo(Type viewModelType)
    {
        var vm = _serviceProvider.GetService(viewModelType) as ViewModelBase
            ?? throw new InvalidOperationException($"ViewModel not registered: {viewModelType.Name}");
        _logger.LogInformation("Navigating to {ViewModel}", viewModelType.Name);
        CurrentViewModel = vm;
    }
}
