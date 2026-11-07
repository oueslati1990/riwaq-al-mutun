using MatnMemorization.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace MatnMemorization.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "10.0.14");

        modelBuilder.Entity("MatnMemorization.Domain.Entities.Matn", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
            b.Property<string>("Author").HasMaxLength(200).HasColumnType("TEXT");
            b.Property<string>("Category").HasMaxLength(100).HasColumnType("TEXT");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("TEXT");
            b.Property<string>("Description").HasMaxLength(1000).HasColumnType("TEXT");
            b.Property<bool>("IsDeleted").HasDefaultValue(false).HasColumnType("INTEGER");
            b.Property<int>("Status").HasColumnType("INTEGER");
            b.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
            b.Property<DateTimeOffset>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("Category");
            b.HasIndex("IsDeleted");
            b.ToTable("Matns");
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.MatnSection", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
            b.Property<Guid>("MatnId").HasColumnType("TEXT");
            b.Property<int>("Order").HasColumnType("INTEGER");
            b.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("MatnId", "Order").IsUnique();
            b.ToTable("MatnSections");
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.TextUnit", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("TEXT");
            b.Property<string>("Content").IsRequired().HasMaxLength(2000).HasColumnType("TEXT");
            b.Property<DateTimeOffset>("CreatedAt").HasColumnType("TEXT");
            b.Property<int>("Order").HasColumnType("INTEGER");
            b.Property<string>("SearchIndex").IsRequired().HasMaxLength(2000).HasColumnType("TEXT");
            b.Property<Guid>("SectionId").HasColumnType("TEXT");
            b.Property<DateTimeOffset>("UpdatedAt").HasColumnType("TEXT");
            b.HasKey("Id");
            b.HasIndex("SearchIndex");
            b.HasIndex("SectionId", "Order").IsUnique();
            b.ToTable("TextUnits");
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.MatnSection", b =>
        {
            b.HasOne("MatnMemorization.Domain.Entities.Matn", null)
             .WithMany("Sections")
             .HasForeignKey("MatnId")
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.TextUnit", b =>
        {
            b.HasOne("MatnMemorization.Domain.Entities.MatnSection", null)
             .WithMany("TextUnits")
             .HasForeignKey("SectionId")
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired();
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.Matn", b =>
        {
            b.Navigation("Sections");
        });

        modelBuilder.Entity("MatnMemorization.Domain.Entities.MatnSection", b =>
        {
            b.Navigation("TextUnits");
        });
#pragma warning restore 612, 618
    }
}
