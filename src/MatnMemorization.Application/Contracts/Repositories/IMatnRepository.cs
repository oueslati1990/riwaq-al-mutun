using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Contracts.Repositories;

public interface IMatnRepository
{
    Task<Matn?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Matn>> GetAllAsync(string? searchTerm = null, string? category = null, CancellationToken ct = default);
    Task AddAsync(Matn matn, CancellationToken ct = default);
    Task UpdateAsync(Matn matn, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
