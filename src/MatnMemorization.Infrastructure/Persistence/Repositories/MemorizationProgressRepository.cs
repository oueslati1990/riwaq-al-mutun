using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence.Repositories;

public sealed class MemorizationProgressRepository(AppDbContext context) : IMemorizationProgressRepository
{
    public async Task<MemorizationProgress?> GetByTextUnitIdAsync(Guid textUnitId, CancellationToken ct = default)
        => await context.MemorizationProgresses
            .Include(p => p.Attempts)
            .FirstOrDefaultAsync(p => p.TextUnitId == textUnitId, ct);

    public async Task<IDictionary<Guid, MemorizationProgress>> GetByTextUnitIdsAsync(
        IEnumerable<Guid> textUnitIds, CancellationToken ct = default)
    {
        var ids = textUnitIds.ToList();
        var rows = await context.MemorizationProgresses
            .Where(p => ids.Contains(p.TextUnitId))
            .ToListAsync(ct);
        return rows.ToDictionary(p => p.TextUnitId);
    }

    public async Task AddAsync(MemorizationProgress progress, CancellationToken ct = default)
        => await context.MemorizationProgresses.AddAsync(progress, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
