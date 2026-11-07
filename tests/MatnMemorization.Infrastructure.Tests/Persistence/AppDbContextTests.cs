using MatnMemorization.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Tests.Persistence;

public class AppDbContextTests
{
    [Fact]
    public void CanCreateDbContext_WithInMemoryProvider()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        using var db = new AppDbContext(options);

        Assert.NotNull(db);
        Assert.True(db.Database.EnsureCreated());
    }

    [Fact]
    public void ModelContainsExpectedEntityTypes()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        using var db = new AppDbContext(options);

        var entityNames = db.Model.GetEntityTypes().Select(e => e.ClrType.Name).ToHashSet();
        Assert.Contains("Matn", entityNames);
        Assert.Contains("MatnSection", entityNames);
        Assert.Contains("TextUnit", entityNames);
    }
}
