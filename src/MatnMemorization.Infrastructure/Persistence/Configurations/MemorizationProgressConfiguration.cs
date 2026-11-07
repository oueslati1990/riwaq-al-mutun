using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatnMemorization.Infrastructure.Persistence.Configurations;

internal sealed class MemorizationProgressConfiguration : IEntityTypeConfiguration<MemorizationProgress>
{
    public void Configure(EntityTypeBuilder<MemorizationProgress> b)
    {
        b.ToTable("MemorizationProgresses");
        b.HasKey(p => p.Id);

        b.Property(p => p.TextUnitId).IsRequired();
        b.Property(p => p.EaseFactor).HasDefaultValue(2.5);
        b.Property(p => p.Interval).HasDefaultValue(0);
        b.Property(p => p.ReviewCount).HasDefaultValue(0);
        b.Property(p => p.NextReviewDate);
        b.Property(p => p.LastReviewedAt);

        b.HasIndex(p => p.TextUnitId).IsUnique();
        b.HasIndex(p => p.NextReviewDate);

        b.HasMany(p => p.Attempts)
         .WithOne()
         .HasForeignKey(a => a.MemorizationProgressId)
         .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(p => p.Attempts)
         .HasField("_attempts")
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
