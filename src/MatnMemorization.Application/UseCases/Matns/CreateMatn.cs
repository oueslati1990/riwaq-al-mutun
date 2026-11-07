using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Matns;

public record CreateMatnCommand(string Title, string? Author, string? Category, string? Description);

public class CreateMatnHandler
{
    private readonly IMatnRepository _repo;

    public CreateMatnHandler(IMatnRepository repo) => _repo = repo;

    public async Task<Matn> HandleAsync(CreateMatnCommand cmd, CancellationToken ct = default)
    {
        var matn = Matn.Create(cmd.Title, cmd.Author, cmd.Category, cmd.Description);
        await _repo.AddAsync(matn, ct);
        await _repo.SaveChangesAsync(ct);
        return matn;
    }
}
