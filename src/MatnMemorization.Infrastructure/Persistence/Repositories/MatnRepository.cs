using MatnMemorization.Application.Contracts.Repositories;
using MatnMemorization.Domain.Entities;
using MatnMemorization.Domain.Utilities;
using Microsoft.EntityFrameworkCore;

namespace MatnMemorization.Infrastructure.Persistence.Repositories;

public class MatnRepository : IMatnRepository
{
    private readonly AppDbContext _db;

    public MatnRepository(AppDbContext db) => _db = db;

    public async Task<Matn?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Matns
                    .Include(m => m.Sections)
                    .ThenInclude(s => s.TextUnits)
                    .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<IReadOnlyList<Matn>> GetAllAsync(string? searchTerm = null, string? category = null, CancellationToken ct = default)
    {
        var query = _db.Matns.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(m => m.Category == category);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalized = ArabicNormalizer.ToSearchIndex(searchTerm);
            // Search by title (EF will use LIKE on SQLite)
            query = query.Where(m =>
                EF.Functions.Like(m.Title, $"%{searchTerm}%") ||
                EF.Functions.Like(m.Title, $"%{normalized}%") ||
                (m.Author != null && EF.Functions.Like(m.Author, $"%{searchTerm}%")));
        }

        return await query
            .OrderBy(m => m.Title)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Matn matn, CancellationToken ct = default)
        => await _db.Matns.AddAsync(matn, ct);

    public Task UpdateAsync(Matn matn, CancellationToken ct = default)
    {
        _db.Matns.Update(matn);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
