using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Matns;

public record GetMatnDetailQuery(Guid MatnId);

public class GetMatnDetailHandler
{
    private readonly IMatnRepository _repo;

    public GetMatnDetailHandler(IMatnRepository repo) => _repo = repo;

    public async Task<Matn?> HandleAsync(GetMatnDetailQuery query, CancellationToken ct = default)
        => await _repo.GetByIdAsync(query.MatnId, ct);
}
