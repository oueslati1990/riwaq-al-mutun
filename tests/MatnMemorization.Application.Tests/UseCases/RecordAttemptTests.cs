using MatnMemorization.Application.Tests.Fakes;
using MatnMemorization.Application.UseCases.Sessions;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Tests.UseCases;

public class RecordAttemptTests
{
    private static RecordAttemptHandler CreateHandler(out FakeMemorizationProgressRepository repo)
    {
        repo = new FakeMemorizationProgressRepository();
        return new RecordAttemptHandler(repo);
    }

    [Fact]
    public async Task RecordAttempt_NewUnit_CreatesProgressRecord()
    {
        var handler = CreateHandler(out var repo);
        var unitId = Guid.NewGuid();

        await handler.HandleAsync(new RecordAttemptCommand(unitId, 5));

        Assert.Single(repo.All);
        Assert.Equal(unitId, repo.All[0].TextUnitId);
    }

    [Fact]
    public async Task RecordAttempt_ExistingUnit_UpdatesExistingProgress()
    {
        var handler = CreateHandler(out var repo);
        var unitId = Guid.NewGuid();

        await handler.HandleAsync(new RecordAttemptCommand(unitId, 5));
        await handler.HandleAsync(new RecordAttemptCommand(unitId, 3));

        Assert.Single(repo.All);
        Assert.Equal(2, repo.All[0].ReviewCount);
    }

    [Fact]
    public async Task RecordAttempt_Quality5_SetsNextReviewDate()
    {
        var handler = CreateHandler(out var repo);

        await handler.HandleAsync(new RecordAttemptCommand(Guid.NewGuid(), 5));

        Assert.NotNull(repo.All[0].NextReviewDate);
    }

    [Fact]
    public async Task RecordAttempt_Quality0_ResetsReviewCount()
    {
        var handler = CreateHandler(out var repo);
        var unitId = Guid.NewGuid();

        // Two successful reviews then a failure
        await handler.HandleAsync(new RecordAttemptCommand(unitId, 5));
        await handler.HandleAsync(new RecordAttemptCommand(unitId, 5));
        await handler.HandleAsync(new RecordAttemptCommand(unitId, 0));

        Assert.Equal(0, repo.All[0].ReviewCount);
    }

    [Fact]
    public async Task RecordAttempt_InvalidQuality_Throws()
    {
        var handler = CreateHandler(out _);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(new RecordAttemptCommand(Guid.NewGuid(), 2)));
    }
}
