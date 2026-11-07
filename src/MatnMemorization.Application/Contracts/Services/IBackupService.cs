namespace MatnMemorization.Application.Contracts.Services;

public interface IBackupService
{
    Task BackupAsync(string destinationPath, CancellationToken ct = default);
    Task<bool> ValidateAsync(string sourcePath, CancellationToken ct = default);
    Task RestoreAsync(string sourcePath, CancellationToken ct = default);
}
