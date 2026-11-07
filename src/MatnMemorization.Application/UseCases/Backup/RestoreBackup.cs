using MatnMemorization.Application.Contracts.Services;

namespace MatnMemorization.Application.UseCases.Backup;

public record RestoreBackupCommand(string SourcePath);

public sealed class RestoreBackupHandler(IBackupService backup)
{
    public async Task HandleAsync(RestoreBackupCommand command, CancellationToken ct = default)
    {
        if (!await backup.ValidateAsync(command.SourcePath, ct))
            throw new InvalidOperationException("الملف غير صالح أو ليس قاعدة بيانات SQLite.");

        await backup.RestoreAsync(command.SourcePath, ct);
    }
}
