namespace MatnMemorization.Domain.Services;

public static class Sm2Calculator
{
    public const double DefaultEaseFactor = 2.5;
    public const double MinEaseFactor = 1.3;

    /// <summary>
    /// Returns (newInterval in days, newEaseFactor).
    /// quality must be 0, 3, or 5.
    /// </summary>
    public static (int NewInterval, double NewEaseFactor) Calculate(
        double easeFactor, int previousInterval, int reviewCount, int quality)
    {
        if (quality < 3)
            return (1, easeFactor); // failure: reset to tomorrow, EF unchanged

        int newInterval = reviewCount switch
        {
            0 => 1,
            1 => 6,
            _ => (int)Math.Round(previousInterval * easeFactor)
        };

        double newEF = easeFactor + 0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02);
        return (newInterval, Math.Max(MinEaseFactor, newEF));
    }
}
