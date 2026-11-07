using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MatnMemorization.Application.Models;
using MatnMemorization.Application.UseCases.Sessions;
using MatnMemorization.Presentation.Wpf.Services;

namespace MatnMemorization.Presentation.Wpf.ViewModels;

public partial class MemorizationViewModel : ViewModelBase
{
    private readonly StartSessionHandler _startSession;
    private readonly RecordAttemptHandler _recordAttempt;
    private readonly MatnSelectionService _selection;

    private List<SessionUnit> _units = [];
    private int _currentIndex;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isNoMatn;
    [ObservableProperty] private bool _isComplete;
    [ObservableProperty] private bool _isTextRevealed;
    [ObservableProperty] private SessionUnit? _currentUnit;
    [ObservableProperty] private int _totalUnits;
    [ObservableProperty] private string? _errorMessage;

    public string SessionProgress => $"{Math.Min(_currentIndex + 1, TotalUnits)} / {TotalUnits}";
    public int CurrentUnitNumber => Math.Min(_currentIndex + 1, TotalUnits);
    public bool IsActiveSession => !IsNoMatn && !IsLoading && !IsComplete && CurrentUnit is not null;

    public MemorizationViewModel(
        StartSessionHandler startSession,
        RecordAttemptHandler recordAttempt,
        MatnSelectionService selection)
    {
        _startSession = startSession;
        _recordAttempt = recordAttempt;
        _selection = selection;
    }

    [RelayCommand]
    private async Task LoadAsync(CancellationToken ct = default)
    {
        if (_selection.SelectedMatnId is null)
        {
            IsNoMatn = true;
            IsComplete = false;
            CurrentUnit = null;
            return;
        }

        IsNoMatn = false;
        IsComplete = false;
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            _units = [.. await _startSession.HandleAsync(
                new StartSessionQuery(_selection.SelectedMatnId.Value), ct)];
            _currentIndex = 0;
            TotalUnits = _units.Count;

            if (_units.Count == 0)
            {
                IsComplete = true;
            }
            else
            {
                ShowCurrent();
            }
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
    private void Reveal()
    {
        IsTextRevealed = true;
    }

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
}
