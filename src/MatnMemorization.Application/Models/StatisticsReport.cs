namespace MatnMemorization.Application.Models;

public sealed record OverallStats(
    int TotalMatns,
    int TotalUnits,
    int StartedUnits,
    int DueTodayCount,
    int TotalReviews);

public sealed record MatnStats(
    Guid MatnId,
    string Title,
    string? Author,
    int TotalUnits,
    int StartedUnits,
    int DueTodayCount,
    int CompletionPercent);

public sealed record StatisticsReport(
    OverallStats Overall,
    IReadOnlyList<MatnStats> Matns);
