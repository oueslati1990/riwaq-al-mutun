using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Contracts.Repositories;

public interface ITextUnitRepository
{
    Task<TextUnit?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(TextUnit unit, CancellationToken ct = default);
    Task UpdateAsync(TextUnit unit, CancellationToken ct = default);
    Task DeleteAsync(TextUnit unit, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
