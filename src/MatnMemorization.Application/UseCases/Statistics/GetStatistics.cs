using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.UseCases.Statistics;

public sealed class GetStatisticsHandler(IStatisticsRepository statsRepo)
{
    public Task<StatisticsReport> HandleAsync(CancellationToken ct = default)
        => statsRepo.GetStatisticsAsync(DateOnly.FromDateTime(DateTime.Today), ct);
}
