using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Contracts.Repositories;

public interface IMatnSectionRepository
{
    Task<MatnSection?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(MatnSection section, CancellationToken ct = default);
    Task UpdateAsync(MatnSection section, CancellationToken ct = default);
    Task DeleteAsync(MatnSection section, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
