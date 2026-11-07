using MatnMemorization.Application.Contracts.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace MatnMemorization.Infrastructure.Backup;

public sealed class BackupService(IOptions<BackupOptions> options) : IBackupService
{
    private static ReadOnlySpan<byte> SqliteMagic => "SQLite format 3\0"u8;

    private BackupOptions Opts => options.Value;

    public async Task BackupAsync(string destinationPath, CancellationToken ct = default)
    {
        var tempPath = destinationPath + ".tmp";
        if (File.Exists(tempPath)) File.Delete(tempPath);

        using var conn = new SqliteConnection(Opts.ConnectionString);
        await conn.OpenAsync(ct);
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "VACUUM INTO $dest";
        cmd.Parameters.AddWithValue("$dest", tempPath);
        await cmd.ExecuteNonQueryAsync(ct);

        File.Move(tempPath, destinationPath, overwrite: true);
    }

    public async Task<bool> ValidateAsync(string sourcePath, CancellationToken ct = default)
    {
        if (!File.Exists(sourcePath)) return false;
        try
        {
            byte[] header = new byte[16];
            await using var fs = File.OpenRead(sourcePath);
            int read = await fs.ReadAsync(header.AsMemory(0, 16), ct);
            return read == 16 && header.AsSpan().SequenceEqual(SqliteMagic);
        }
        catch
        {
            return false;
        }
    }

    public async Task RestoreAsync(string sourcePath, CancellationToken ct = default)
    {
        // Close all pooled SQLite connections so the file can be replaced
        SqliteConnection.ClearAllPools();
        await Task.Run(() => File.Copy(sourcePath, Opts.DatabasePath, overwrite: true), ct);
    }
}
