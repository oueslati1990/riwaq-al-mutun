using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Models;
using MatnMemorization.Application.UseCases.Statistics;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class StatisticsViewModel : ViewModelBase
{
    private readonly GetStatisticsHandler _getStats;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private OverallStats? _overall;
    [ObservableProperty] private IReadOnlyList<MatnStats> _matnStats = [];
    [ObservableProperty] private string? _errorMessage;

    public bool IsEmpty => !IsLoading && (Overall?.TotalMatns ?? 0) == 0;

    public StatisticsViewModel(GetStatisticsHandler getStats)
    {
        _getStats = getStats;
    }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var report = await _getStats.HandleAsync(ct);
            Overall = report.Overall;
            MatnStats = report.Matns;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }
}
