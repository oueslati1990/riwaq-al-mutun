using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.Tests.Fakes;

public sealed class FakeStatisticsRepository(StatisticsReport report) : IStatisticsRepository
{
    public Task<StatisticsReport> GetStatisticsAsync(DateOnly today, CancellationToken ct = default)
        => Task.FromResult(report);
}
