namespace MatnMemorization.Domain.Entities;

public class ReviewAttempt
{
    public Guid Id { get; private set; }
    public Guid MemorizationProgressId { get; private set; }
    public int Quality { get; private set; }
    public DateTimeOffset ReviewedAt { get; private set; }

#pragma warning disable CS8618
    private ReviewAttempt() { }
#pragma warning restore CS8618

    internal static ReviewAttempt Create(Guid progressId, int quality, DateTimeOffset reviewedAt) => new()
    {
        Id = Guid.NewGuid(),
        MemorizationProgressId = progressId,
        Quality = quality,
        ReviewedAt = reviewedAt
    };
}
