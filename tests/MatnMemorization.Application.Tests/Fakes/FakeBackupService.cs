using MatnMemorization.Application.Contracts.Services;

namespace MatnMemorization.Application.Tests.Fakes;

public sealed class FakeBackupService : IBackupService
{
    public string? LastBackupDestination { get; private set; }
    public string? LastRestoreSource { get; private set; }
    public bool ValidationResult { get; set; } = true;

    public Task BackupAsync(string destinationPath, CancellationToken ct = default)
    {
        LastBackupDestination = destinationPath;
        return Task.CompletedTask;
    }

    public Task<bool> ValidateAsync(string sourcePath, CancellationToken ct = default)
        => Task.FromResult(ValidationResult);

    public Task RestoreAsync(string sourcePath, CancellationToken ct = default)
    {
        LastRestoreSource = sourcePath;
        return Task.CompletedTask;
    }
}
