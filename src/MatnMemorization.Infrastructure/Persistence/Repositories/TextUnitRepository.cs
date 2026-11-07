using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence.Repositories;

public class TextUnitRepository : ITextUnitRepository
{
    private readonly AppDbContext _db;

    public TextUnitRepository(AppDbContext db) => _db = db;

    public async Task<TextUnit?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.TextUnits.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(TextUnit unit, CancellationToken ct = default)
        => await _db.TextUnits.AddAsync(unit, ct);

    public Task UpdateAsync(TextUnit unit, CancellationToken ct = default)
    {
        _db.TextUnits.Update(unit);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TextUnit unit, CancellationToken ct = default)
    {
        _db.TextUnits.Remove(unit);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
