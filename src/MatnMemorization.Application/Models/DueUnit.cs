namespace MatnMemorization.Application.Models;

public sealed record DueUnit(
    Guid TextUnitId,
    string Content,
    string SectionTitle,
    string MatnTitle,
    DateOnly NextReviewDate);
