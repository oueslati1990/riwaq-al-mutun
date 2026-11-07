using MatnMemorization.Application.Models;
using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Statistics;

namespace MatnMemorization.Application.Tests.UseCases;

public class GetStatisticsTests
{
    private static StatisticsReport MakeReport(int totalMatns, int totalUnits, int started, int dueToday, int reviews,
        IReadOnlyList<MatnStats>? matns = null)
        => new(new OverallStats(totalMatns, totalUnits, started, dueToday, reviews),
               matns ?? []);

    [Fact]
    public async Task GetStatistics_ReturnsReportFromRepository()
    {
        var expected = MakeReport(2, 30, 15, 5, 42);
        var handler = new GetStatisticsHandler(new FakeStatisticsRepository(expected));

        var result = await handler.HandleAsync();

        Assert.Equal(2, result.Overall.TotalMatns);
        Assert.Equal(30, result.Overall.TotalUnits);
        Assert.Equal(15, result.Overall.StartedUnits);
        Assert.Equal(5, result.Overall.DueTodayCount);
        Assert.Equal(42, result.Overall.TotalReviews);
    }

    [Fact]
    public async Task GetStatistics_EmptyDb_ReturnsZeroCounts()
    {
        var handler = new GetStatisticsHandler(new FakeStatisticsRepository(MakeReport(0, 0, 0, 0, 0)));

        var result = await handler.HandleAsync();

        Assert.Equal(0, result.Overall.TotalMatns);
        Assert.Equal(0, result.Overall.TotalUnits);
        Assert.Empty(result.Matns);
    }

    [Fact]
    public async Task GetStatistics_MatnStats_CorrectCompletionPercent()
    {
        var matnStats = new List<MatnStats>
        {
            new(Guid.NewGuid(), "الآجرومية", null, 10, 10, 0, 100),
            new(Guid.NewGuid(), "المتن الثاني", null, 10, 5, 2, 50),
            new(Guid.NewGuid(), "متن فارغ", null, 0, 0, 0, 0),
        };
        var handler = new GetStatisticsHandler(
            new FakeStatisticsRepository(MakeReport(3, 20, 15, 2, 20, matnStats)));

        var result = await handler.HandleAsync();

        Assert.Equal(100, result.Matns[0].CompletionPercent);
        Assert.Equal(50, result.Matns[1].CompletionPercent);
        Assert.Equal(0, result.Matns[2].CompletionPercent);
    }
}
