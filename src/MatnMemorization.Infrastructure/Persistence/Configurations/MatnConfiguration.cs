using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MatnMemorization.Infrastructure.Persistence.Configurations;

public class MatnConfiguration : IEntityTypeConfiguration<Matn>
{
    public void Configure(EntityTypeBuilder<Matn> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(m => m.Author).HasMaxLength(200);
        builder.Property(m => m.Category).HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(1000);
        builder.Property(m => m.Status).IsRequired();
        builder.Property(m => m.IsDeleted).IsRequired().HasDefaultValue(false);
        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.UpdatedAt).IsRequired();

        builder.HasIndex(m => m.IsDeleted);
        builder.HasIndex(m => m.Category);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.HasMany(m => m.Sections)
               .WithOne()
               .HasForeignKey(s => s.MatnId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
