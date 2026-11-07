using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.Contracts.Repositories;

public interface IStatisticsRepository
{
    Task<StatisticsReport> GetStatisticsAsync(DateOnly today, CancellationToken ct = default);
}
