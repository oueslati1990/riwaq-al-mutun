namespace MatnMemorization.Application.Contracts.Navigation;

public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
    void NavigateTo(Type viewModelType);
}
