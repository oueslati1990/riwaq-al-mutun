namespace MatnMemorization.Infrastructure.Backup;

public sealed class BackupOptions
{
    public string DatabasePath { get; set; } = string.Empty;
    public string ConnectionString => $"Data Source={DatabasePath}";
}
