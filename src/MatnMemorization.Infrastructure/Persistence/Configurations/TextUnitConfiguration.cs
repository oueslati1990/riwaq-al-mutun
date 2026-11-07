using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatnMemorization.Infrastructure.Persistence.Configurations;

public class TextUnitConfiguration : IEntityTypeConfiguration<TextUnit>
{
    public void Configure(EntityTypeBuilder<TextUnit> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Content)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(u => u.SearchIndex)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(u => u.Order).IsRequired();
        builder.Property(u => u.CreatedAt).IsRequired();
        builder.Property(u => u.UpdatedAt).IsRequired();

        builder.HasIndex(u => new { u.SectionId, u.Order }).IsUnique();
        builder.HasIndex(u => u.SearchIndex);
    }
}
