using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatnMemorization.Infrastructure.Persistence.Configurations;

public class MatnSectionConfiguration : IEntityTypeConfiguration<MatnSection>
{
    public void Configure(EntityTypeBuilder<MatnSection> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(s => s.Order).IsRequired();

        builder.HasIndex(s => new { s.MatnId, s.Order }).IsUnique();

        builder.HasMany(s => s.TextUnits)
               .WithOne()
               .HasForeignKey(u => u.SectionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
