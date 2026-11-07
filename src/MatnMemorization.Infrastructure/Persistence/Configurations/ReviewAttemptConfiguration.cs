using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatnMemorization.Infrastructure.Persistence.Configurations;

internal sealed class ReviewAttemptConfiguration : IEntityTypeConfiguration<ReviewAttempt>
{
    public void Configure(EntityTypeBuilder<ReviewAttempt> b)
    {
        b.ToTable("ReviewAttempts");
        b.HasKey(a => a.Id);

        b.Property(a => a.MemorizationProgressId).IsRequired();
        b.Property(a => a.Quality).IsRequired();
        b.Property(a => a.ReviewedAt).IsRequired();

        b.HasIndex(a => a.MemorizationProgressId);
    }
}
