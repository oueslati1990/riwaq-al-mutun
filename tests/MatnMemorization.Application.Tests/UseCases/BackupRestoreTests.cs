using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Backup;

namespace MatnMemorization.Application.Tests.UseCases;

public class BackupRestoreTests
{
    [Fact]
    public async Task CreateBackup_PassesDestinationToService()
    {
        var fake = new FakeBackupService();
        var handler = new CreateBackupHandler(fake);

        await handler.HandleAsync(new CreateBackupCommand("/tmp/backup.db"));

        Assert.Equal("/tmp/backup.db", fake.LastBackupDestination);
    }

    [Fact]
    public async Task RestoreBackup_ValidFile_CallsRestoreService()
    {
        var fake = new FakeBackupService { ValidationResult = true };
        var handler = new RestoreBackupHandler(fake);

        await handler.HandleAsync(new RestoreBackupCommand("/tmp/backup.db"));

        Assert.Equal("/tmp/backup.db", fake.LastRestoreSource);
    }

    [Fact]
    public async Task RestoreBackup_InvalidFile_ThrowsInvalidOperationException()
    {
        var fake = new FakeBackupService { ValidationResult = false };
        var handler = new RestoreBackupHandler(fake);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new RestoreBackupCommand("/tmp/corrupt.db")));

        Assert.Null(fake.LastRestoreSource);
    }

    [Fact]
    public async Task RestoreBackup_InvalidFile_DoesNotCallRestore()
    {
        var fake = new FakeBackupService { ValidationResult = false };
        var handler = new RestoreBackupHandler(fake);

        try { await handler.HandleAsync(new RestoreBackupCommand("/tmp/x.db")); }
        catch (InvalidOperationException) { }

        Assert.Null(fake.LastRestoreSource);
    }
}
