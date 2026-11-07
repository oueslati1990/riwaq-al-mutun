using MatnMemorization.Application.Contracts.Repositories;

namespace MatnMemorization.Application.UseCases.Matns;

public record UpdateMatnCommand(Guid Id, string Title, string? Author, string? Category, string? Description);

public class UpdateMatnHandler
{
    private readonly IMatnRepository _repo;

    public UpdateMatnHandler(IMatnRepository repo) => _repo = repo;

    public async Task HandleAsync(UpdateMatnCommand cmd, CancellationToken ct = default)
    {
        var matn = await _repo.GetByIdAsync(cmd.Id, ct)
            ?? throw new InvalidOperationException($"Matn {cmd.Id} not found.");
        matn.Update(cmd.Title, cmd.Author, cmd.Category, cmd.Description);
        await _repo.UpdateAsync(matn, ct);
        await _repo.SaveChangesAsync(ct);
    }
}
