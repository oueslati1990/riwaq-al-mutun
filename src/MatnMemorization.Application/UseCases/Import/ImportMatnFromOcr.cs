using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Application.Models;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Import;

public record ImportMatnFromOcrCommand(
    string Title,
    string? Author,
    IReadOnlyList<OcrPageResult> ApprovedPages);

public class ImportMatnFromOcrHandler(IMatnRepository matnRepo)
{
    public async Task<Guid> HandleAsync(ImportMatnFromOcrCommand command, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(command.Title))
            throw new ArgumentException("العنوان مطلوب.", nameof(command));

        var matn = Matn.Create(command.Title, command.Author);

        foreach (var page in command.ApprovedPages)
        {
            var text = page.EditedText.Trim();
            if (string.IsNullOrWhiteSpace(text)) continue;

            var section = matn.AddSection($"صفحة {page.PageNumber}");

            var lines = text.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    section.AddTextUnit(line);
            }
        }

        await matnRepo.AddAsync(matn, ct);
        await matnRepo.SaveChangesAsync(ct);
        return matn.Id;
    }
}
