using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;

namespace MatnMemorization.Application.UseCases.Review;

public record GetDueUnitsQuery(DateOnly Date);

public sealed class GetDueUnitsHandler(IMemorizationProgressRepository progressRepo)
{
    public Task<IReadOnlyList<DueUnit>> HandleAsync(GetDueUnitsQuery query, CancellationToken ct = default)
        => progressRepo.GetDueUnitsAsync(query.Date, ct);
}
