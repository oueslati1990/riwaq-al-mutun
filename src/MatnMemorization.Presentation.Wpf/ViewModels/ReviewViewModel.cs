using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Models;
using MatnMemorization.Application.UseCases.Review;
using MatnMemorization.Application.UseCases.Sessions;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class ReviewViewModel : ViewModelBase
{
    private readonly GetDueUnitsHandler _getDueUnits;
    private readonly RecordAttemptHandler _recordAttempt;

    private List<DueUnit> _units = [];
    private int _currentIndex;
    private int _ratedCount;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isNothingDue;
    [ObservableProperty] private bool _isComplete;
    [ObservableProperty] private bool _isTextRevealed;
    [ObservableProperty] private DueUnit? _currentUnit;
    [ObservableProperty] private int _totalUnits;
    [ObservableProperty] private string? _errorMessage;

    public string SessionProgress => $"{Math.Min(_currentIndex + 1, TotalUnits)} / {TotalUnits}";
    public int CurrentUnitNumber => Math.Min(_currentIndex + 1, TotalUnits);
    public bool IsActiveSession => !IsNothingDue && !IsLoading && !IsComplete && CurrentUnit is not null;

    public ReviewViewModel(GetDueUnitsHandler getDueUnits, RecordAttemptHandler recordAttempt)
    {
        _getDueUnits = getDueUnits;
        _recordAttempt = recordAttempt;
    }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        IsNothingDue = false;
        IsComplete = false;
        ErrorMessage = null;
        _ratedCount = 0;

        try
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            _units = [.. await _getDueUnits.HandleAsync(new GetDueUnitsQuery(today), ct)];
            _currentIndex = 0;
            TotalUnits = _units.Count;

            if (_units.Count == 0)
                IsNothingDue = true;
            else
                ShowCurrent();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsActiveSession));
        }
    }

    [RelayCommand]
    private void Reveal() => IsTextRevealed = true;

    [RelayCommand]
    private async Task RateForgotAsync(CancellationToken ct = default) => await RateAsync(0, ct);

    [RelayCommand]
    private async Task RateHardAsync(CancellationToken ct = default) => await RateAsync(3, ct);

    [RelayCommand]
    private async Task RateEasyAsync(CancellationToken ct = default) => await RateAsync(5, ct);

    [RelayCommand]
    private async Task RestartAsync(CancellationToken ct = default) => await LoadAsync(ct);

    private async Task RateAsync(int quality, CancellationToken ct)
    {
        if (CurrentUnit is null) return;

        ErrorMessage = null;
        try
        {
            await _recordAttempt.HandleAsync(
                new RecordAttemptCommand(CurrentUnit.TextUnitId, quality), ct);
            _ratedCount++;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        Advance();
    }

    private void Advance()
    {
        _currentIndex++;
        if (_currentIndex >= _units.Count)
        {
            IsComplete = true;
            CurrentUnit = null;
            OnPropertyChanged(nameof(IsActiveSession));
            OnPropertyChanged(nameof(RatedCount));
        }
        else
        {
            ShowCurrent();
        }
    }

    private void ShowCurrent()
    {
        CurrentUnit = _units[_currentIndex];
        IsTextRevealed = false;
        OnPropertyChanged(nameof(SessionProgress));
        OnPropertyChanged(nameof(CurrentUnitNumber));
        OnPropertyChanged(nameof(IsActiveSession));
    }

    public int RatedCount => _ratedCount;
}
