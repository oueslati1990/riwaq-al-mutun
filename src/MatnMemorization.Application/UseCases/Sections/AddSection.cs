using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Sections;

public record AddSectionCommand(Guid MatnId, string Title);

public class AddSectionHandler
{
    private readonly IMatnRepository _repo;

    public AddSectionHandler(IMatnRepository repo) => _repo = repo;

    public async Task<MatnSection> HandleAsync(AddSectionCommand cmd, CancellationToken ct = default)
    {
        var matn = await _repo.GetByIdAsync(cmd.MatnId, ct)
            ?? throw new InvalidOperationException($"Matn {cmd.MatnId} not found.");
        var section = matn.AddSection(cmd.Title);
        await _repo.UpdateAsync(matn, ct);
        await _repo.SaveChangesAsync(ct);
        return section;
    }
}
