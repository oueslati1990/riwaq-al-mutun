using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.UseCases.Sessions;

public record RecordAttemptCommand(Guid TextUnitId, int Quality);

public sealed class RecordAttemptHandler(IMemorizationProgressRepository progressRepo)
{
    public async Task HandleAsync(RecordAttemptCommand command, CancellationToken ct = default)
    {
        if (command.Quality != 0 && command.Quality != 3 && command.Quality != 5)
            throw new ArgumentException("Quality must be 0, 3, or 5.", nameof(command));

        var progress = await progressRepo.GetByTextUnitIdAsync(command.TextUnitId, ct);
        if (progress is null)
        {
            progress = MemorizationProgress.Create(command.TextUnitId);
            await progressRepo.AddAsync(progress, ct);
        }

        progress.RecordAttempt(command.Quality, DateTimeOffset.UtcNow);
        await progressRepo.SaveChangesAsync(ct);
    }
}
