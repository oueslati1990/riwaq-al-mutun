using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;

namespace MatnMemorization.Application.Tests.Fakes;

internal sealed class FakeMatnRepository : IMatnRepository
{
    public List<Matn> Stored { get; } = [];

    public Task AddAsync(Matn matn, CancellationToken ct = default)
    {
        Stored.Add(matn);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task<Matn?> GetByIdAsync(Guid id, CancellationToken ct = default) => Task.FromResult<Matn?>(null);
    public Task<IReadOnlyList<Matn>> GetAllAsync(string? searchTerm = null, string? category = null, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Matn>>([]);
    public Task UpdateAsync(Matn matn, CancellationToken ct = default) => Task.CompletedTask;
}
