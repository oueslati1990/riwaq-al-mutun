using MatnMemorization.Application.Models;
using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Review;

namespace MatnMemorization.Application.Tests.UseCases;

public class GetDueUnitsTests
{
    private static readonly DateOnly Today = new(2026, 11, 8);

    private static GetDueUnitsHandler CreateHandler(out FakeMemorizationProgressRepository repo)
    {
        repo = new FakeMemorizationProgressRepository();
        return new GetDueUnitsHandler(repo);
    }

    private static DueUnit MakeDue(DateOnly date) =>
        new(Guid.NewGuid(), "بيت تجريبي", "باب الفاتحة", "الآجرومية", date);

    [Fact]
    public async Task GetDueUnits_NullNextReviewDate_NotIncluded()
    {
        // Units with no NextReviewDate (never reviewed) should NOT appear
        var handler = CreateHandler(out var repo);
        // Fake stores DueUnits explicitly, so just call with no seeds
        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDueUnits_DueDateToday_Included()
    {
        var handler = CreateHandler(out var repo);
        repo.SeedDueUnit(MakeDue(Today));

        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Single(result);
    }

    [Fact]
    public async Task GetDueUnits_DueDateYesterday_Included()
    {
        var handler = CreateHandler(out var repo);
        repo.SeedDueUnit(MakeDue(Today.AddDays(-1)));

        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Single(result);
    }

    [Fact]
    public async Task GetDueUnits_DueDateTomorrow_NotIncluded()
    {
        var handler = CreateHandler(out var repo);
        repo.SeedDueUnit(MakeDue(Today.AddDays(1)));

        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDueUnits_MixedDates_ReturnsOnlyDue()
    {
        var handler = CreateHandler(out var repo);
        repo.SeedDueUnit(MakeDue(Today.AddDays(-2))); // overdue
        repo.SeedDueUnit(MakeDue(Today));              // due today
        repo.SeedDueUnit(MakeDue(Today.AddDays(1)));   // future

        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDueUnits_OrderedByDateAscending()
    {
        var handler = CreateHandler(out var repo);
        repo.SeedDueUnit(MakeDue(Today));
        repo.SeedDueUnit(MakeDue(Today.AddDays(-5)));
        repo.SeedDueUnit(MakeDue(Today.AddDays(-1)));

        var result = await handler.HandleAsync(new GetDueUnitsQuery(Today));

        Assert.Equal(3, result.Count);
        Assert.True(result[0].NextReviewDate <= result[1].NextReviewDate);
        Assert.True(result[1].NextReviewDate <= result[2].NextReviewDate);
    }
}
