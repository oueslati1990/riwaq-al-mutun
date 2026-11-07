namespace MatnMemorization.Application.Models;

public sealed record SessionUnit(
    Guid TextUnitId,
    string Content,
    string SectionTitle,
    int SectionOrder,
    int UnitOrder,
    bool HasProgress,
    DateOnly? NextReviewDate);
