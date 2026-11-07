using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Matn> Matns => Set<Matn>();
    public DbSet<MatnSection> MatnSections => Set<MatnSection>();
    public DbSet<TextUnit> TextUnits => Set<TextUnit>();
    public DbSet<MemorizationProgress> MemorizationProgresses => Set<MemorizationProgress>();
    public DbSet<ReviewAttempt> ReviewAttempts => Set<ReviewAttempt>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
