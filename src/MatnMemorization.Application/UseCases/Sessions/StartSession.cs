using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.UseCases.Sessions;

public record StartSessionQuery(Guid MatnId, int MaxUnits = 20);

public sealed class StartSessionHandler(IMatnRepository matns, IMemorizationProgressRepository progressRepo)
{
    public async Task<IReadOnlyList<SessionUnit>> HandleAsync(StartSessionQuery query, CancellationToken ct = default)
    {
        var matn = await matns.GetByIdAsync(query.MatnId, ct)
            ?? throw new KeyNotFoundException($"المتن غير موجود: {query.MatnId}");

        var flat = matn.Sections
            .OrderBy(s => s.Order)
            .SelectMany(s => s.TextUnits.OrderBy(u => u.Order).Select(u => (Section: s, Unit: u)))
            .Take(query.MaxUnits)
            .ToList();

        var ids = flat.Select(x => x.Unit.Id);
        var progressMap = await progressRepo.GetByTextUnitIdsAsync(ids, ct);

        return flat.Select(x =>
        {
            progressMap.TryGetValue(x.Unit.Id, out var prog);
            return new SessionUnit(
                x.Unit.Id,
                x.Unit.Content,
                x.Section.Title,
                x.Section.Order,
                x.Unit.Order,
                prog is not null,
                prog?.NextReviewDate);
        }).ToList();
    }
}
