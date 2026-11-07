using MatnMemorization.Application.Contracts.Repositories;

namespace MatnMemorization.Application.UseCases.Matns;

public record SoftDeleteMatnCommand(Guid Id);

public class SoftDeleteMatnHandler
{
    private readonly IMatnRepository _repo;

    public SoftDeleteMatnHandler(IMatnRepository repo) => _repo = repo;

    public async Task HandleAsync(SoftDeleteMatnCommand cmd, CancellationToken ct = default)
    {
        var matn = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new InvalidOperationException($"Matn {cmd.Id} not found.");
        matn.SoftDelete();
        await _repo.UpdateAsync(matn, ct);
        await _repo.SaveChangesAsync(ct);
    }
}
