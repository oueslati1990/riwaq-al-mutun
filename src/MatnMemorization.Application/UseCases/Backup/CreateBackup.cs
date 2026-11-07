using MatnMemorization.Application.Contracts.Services;

namespace MatnMemorization.Application.UseCases.Backup;

public record CreateBackupCommand(string DestinationPath);

public sealed class CreateBackupHandler(IBackupService backup)
{
    public Task HandleAsync(CreateBackupCommand command, CancellationToken ct = default)
        => backup.BackupAsync(command.DestinationPath, ct);
}
