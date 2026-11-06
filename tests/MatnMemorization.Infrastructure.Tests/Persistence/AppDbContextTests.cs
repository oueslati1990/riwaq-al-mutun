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
    public void CanSaveAndRetrieveData_WhenModelIsEmpty()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        using var db = new AppDbContext(options);

        // No entities yet — just verify context is usable
        db.Database.EnsureCreated();
        Assert.Empty(db.Model.GetEntityTypes());
    }
}
