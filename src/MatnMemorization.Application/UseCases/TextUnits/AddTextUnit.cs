using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.TextUnits;

public record AddTextUnitCommand(Guid SectionId, string Content);

public class AddTextUnitHandler
{
    private readonly IMatnSectionRepository _sectionRepo;

    public AddTextUnitHandler(IMatnSectionRepository sectionRepo) => _sectionRepo = sectionRepo;

    public async Task<TextUnit> HandleAsync(AddTextUnitCommand cmd, CancellationToken ct = default)
    {
        var section = await _sectionRepo.GetByIdAsync(cmd.SectionId, ct)
            ?? throw new InvalidOperationException($"Section {cmd.SectionId} not found.");
        var unit = section.AddTextUnit(cmd.Content);
        await _sectionRepo.UpdateAsync(section, ct);
        await _sectionRepo.SaveChangesAsync(ct);
        return unit;
    }
}
