using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Tests.Fakes;

public sealed class FakeMemorizationProgressRepository : IMemorizationProgressRepository
{
    private readonly List<MemorizationProgress> _store = [];
    private readonly List<DueUnit> _dueUnits = [];

    public Task<MemorizationProgress?> GetByTextUnitIdAsync(Guid textUnitId, CancellationToken ct = default)
        => Task.FromResult(_store.FirstOrDefault(p => p.TextUnitId == textUnitId));

    public Task<IDictionary<Guid, MemorizationProgress>> GetByTextUnitIdsAsync(
        IEnumerable<Guid> textUnitIds, CancellationToken ct = default)
    {
        var ids = textUnitIds.ToHashSet();
        IDictionary<Guid, MemorizationProgress> result =
            _store.Where(p => ids.Contains(p.TextUnitId)).ToDictionary(p => p.TextUnitId);
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<DueUnit>> GetDueUnitsAsync(DateOnly today, CancellationToken ct = default)
    {
        IReadOnlyList<DueUnit> result = _dueUnits
            .Where(d => d.NextReviewDate <= today)
            .OrderBy(d => d.NextReviewDate)
            .ToList();
        return Task.FromResult(result);
    }

    public Task AddAsync(MemorizationProgress progress, CancellationToken ct = default)
    {
        _store.Add(progress);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;

    public IReadOnlyList<MemorizationProgress> All => _store;

    public void SeedDueUnit(DueUnit unit) => _dueUnits.Add(unit);
}
