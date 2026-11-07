using MatnMemorization.Application.Models;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Contracts.Repositories;

public interface IMemorizationProgressRepository
{
    Task<MemorizationProgress?> GetByTextUnitIdAsync(Guid textUnitId, CancellationToken ct = default);
    Task<IDictionary<Guid, MemorizationProgress>> GetByTextUnitIdsAsync(IEnumerable<Guid> textUnitIds, CancellationToken ct = default);
    Task<IReadOnlyList<DueUnit>> GetDueUnitsAsync(DateOnly today, CancellationToken ct = default);
    Task AddAsync(MemorizationProgress progress, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
