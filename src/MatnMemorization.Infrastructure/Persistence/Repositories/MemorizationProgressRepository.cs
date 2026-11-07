using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;
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

    public async Task<IReadOnlyList<DueUnit>> GetDueUnitsAsync(DateOnly today, CancellationToken ct = default)
    {
        var query =
            from p in context.MemorizationProgresses
            where p.NextReviewDate.HasValue && p.NextReviewDate.Value <= today
            join u in context.TextUnits on p.TextUnitId equals u.Id
            join s in context.MatnSections on u.SectionId equals s.Id
            join m in context.Matns on s.MatnId equals m.Id
            orderby p.NextReviewDate ascending
            select new DueUnit(p.TextUnitId, u.Content, s.Title, m.Title, p.NextReviewDate!.Value);

        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(MemorizationProgress progress, CancellationToken ct = default)
        => await context.MemorizationProgresses.AddAsync(progress, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
