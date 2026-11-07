using MatnMemorization.Domain.Services;

namespace MatnMemorization.Domain.Tests.Services;

public class Sm2CalculatorTests
{
    [Fact]
    public void Calculate_Quality0_ResetsIntervalToOne()
    {
        var (interval, ef) = Sm2Calculator.Calculate(2.5, 10, 5, 0);

        Assert.Equal(1, interval);
        Assert.Equal(2.5, ef); // EF unchanged on failure
    }

    [Fact]
    public void Calculate_FirstSuccessfulReview_IntervalIsOne()
    {
        var (interval, _) = Sm2Calculator.Calculate(2.5, 0, 0, 5);

        Assert.Equal(1, interval);
    }

    [Fact]
    public void Calculate_SecondSuccessfulReview_IntervalIsSix()
    {
        var (interval, _) = Sm2Calculator.Calculate(2.5, 1, 1, 5);

        Assert.Equal(6, interval);
    }

    [Fact]
    public void Calculate_ThirdReviewEasy_IntervalGrowsByEaseFactor()
    {
        // prev=6, EF=2.5, reviewCount=2 → 6 * 2.5 = 15
        var (interval, _) = Sm2Calculator.Calculate(2.5, 6, 2, 5);

        Assert.Equal(15, interval);
    }

    [Fact]
    public void Calculate_Quality5_IncreasesEaseFactor()
    {
        var (_, ef) = Sm2Calculator.Calculate(2.5, 1, 1, 5);

        Assert.True(ef > 2.5);
    }

    [Fact]
    public void Calculate_Quality3_DecreasesEaseFactor()
    {
        var (_, ef) = Sm2Calculator.Calculate(2.5, 1, 1, 3);

        Assert.True(ef < 2.5);
    }

    [Fact]
    public void Calculate_EaseFactorNeverDropsBelowMinimum()
    {
        // After many quality-3 reviews the EF should stay at 1.3
        double ef = 2.5;
        int interval = 0;
        for (int i = 0; i < 20; i++)
        {
            int count = i;
            (interval, ef) = Sm2Calculator.Calculate(ef, interval, count, 3);
        }

        Assert.True(ef >= Sm2Calculator.MinEaseFactor);
    }
}
