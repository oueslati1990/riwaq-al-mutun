using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence.Repositories;

public class MatnSectionRepository : IMatnSectionRepository
{
    private readonly AppDbContext _db;

    public MatnSectionRepository(AppDbContext db) => _db = db;

    public async Task<MatnSection?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.MatnSections
                    .Include(s => s.TextUnits)
                    .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task AddAsync(MatnSection section, CancellationToken ct = default)
        => await _db.MatnSections.AddAsync(section, ct);

    public Task UpdateAsync(MatnSection section, CancellationToken ct = default)
    {
        _db.MatnSections.Update(section);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(MatnSection section, CancellationToken ct = default)
    {
        _db.MatnSections.Remove(section);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
