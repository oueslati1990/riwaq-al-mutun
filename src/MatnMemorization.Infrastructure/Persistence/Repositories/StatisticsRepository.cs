using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence.Repositories;

public sealed class StatisticsRepository(AppDbContext context) : IStatisticsRepository
{
    public async Task<StatisticsReport> GetStatisticsAsync(DateOnly today, CancellationToken ct = default)
    {
        // Matn list with total unit count per matn
        var matns = await context.Matns
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Author,
                TotalUnits = m.Sections.SelectMany(s => s.TextUnits).Count()
            })
            .ToListAsync(ct);

        // Progress entries with their owning Matn id
        var progressByMatn = await (
            from p in context.MemorizationProgresses
            join u in context.TextUnits on p.TextUnitId equals u.Id
            join s in context.MatnSections on u.SectionId equals s.Id
            select new { MatnId = s.MatnId, p.ReviewCount, p.NextReviewDate }
        ).ToListAsync(ct);

        var totalReviews = await context.ReviewAttempts.CountAsync(ct);

        var lookup = progressByMatn.ToLookup(x => x.MatnId);

        var matnStats = matns.Select(m =>
        {
            var progs = lookup[m.Id].ToList();
            int started = progs.Count(p => p.ReviewCount > 0);
            int dueToday = progs.Count(p => p.NextReviewDate.HasValue && p.NextReviewDate.Value <= today);
            int pct = m.TotalUnits == 0 ? 0 : (int)Math.Round(100.0 * started / m.TotalUnits);
            return new MatnStats(m.Id, m.Title, m.Author, m.TotalUnits, started, dueToday, pct);
        }).ToList();

        var overall = new OverallStats(
            matns.Count,
            matns.Sum(m => m.TotalUnits),
            matnStats.Sum(s => s.StartedUnits),
            matnStats.Sum(s => s.DueTodayCount),
            totalReviews);

        return new StatisticsReport(overall, matnStats);
    }
}
