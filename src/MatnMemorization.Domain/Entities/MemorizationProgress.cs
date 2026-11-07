using MatnMemorization.Domain.Services;

namespace MatnMemorization.Domain.Entities;

public class MemorizationProgress
{
    private readonly List<ReviewAttempt> _attempts = [];

    public Guid Id { get; private set; }
    public Guid TextUnitId { get; private set; }
    public double EaseFactor { get; private set; } = Sm2Calculator.DefaultEaseFactor;
    public int Interval { get; private set; }
    public int ReviewCount { get; private set; }
    public DateOnly? NextReviewDate { get; private set; }
    public DateTimeOffset? LastReviewedAt { get; private set; }
    public IReadOnlyList<ReviewAttempt> Attempts => _attempts;

#pragma warning disable CS8618
    private MemorizationProgress() { }
#pragma warning restore CS8618

    public static MemorizationProgress Create(Guid textUnitId) => new()
    {
        Id = Guid.NewGuid(),
        TextUnitId = textUnitId,
        EaseFactor = Sm2Calculator.DefaultEaseFactor,
        Interval = 0,
        ReviewCount = 0
    };

    public void RecordAttempt(int quality, DateTimeOffset now)
    {
        if (quality != 0 && quality != 3 && quality != 5)
            throw new ArgumentException($"Quality must be 0, 3, or 5. Got: {quality}", nameof(quality));

        var (newInterval, newEF) = Sm2Calculator.Calculate(EaseFactor, Interval, ReviewCount, quality);

        EaseFactor = newEF;
        Interval = newInterval;
        ReviewCount = quality >= 3 ? ReviewCount + 1 : 0;
        NextReviewDate = DateOnly.FromDateTime(now.UtcDateTime.Date.AddDays(newInterval));
        LastReviewedAt = now;

        _attempts.Add(ReviewAttempt.Create(Id, quality, now));
    }
}
