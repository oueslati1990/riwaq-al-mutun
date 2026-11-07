using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Matns;

public record GetMatnListQuery(string? SearchTerm = null, string? Category = null);

public class GetMatnListHandler
{
    private readonly IMatnRepository _repo;

    public GetMatnListHandler(IMatnRepository repo) => _repo = repo;

    public async Task<IReadOnlyList<Matn>> HandleAsync(GetMatnListQuery query, CancellationToken ct = default)
        => await _repo.GetAllAsync(query.SearchTerm, query.Category, ct);
}
