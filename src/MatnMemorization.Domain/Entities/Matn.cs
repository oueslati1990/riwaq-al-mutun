using MatnMemorization.Domain.Enums;

namespace MatnMemorization.Domain.Entities;

public class Matn
{
    private readonly List<MatnSection> _sections = [];
    private string _title = default!;

    public Guid Id { get; private set; }

    public string Title
    {
        get => _title;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Matn title cannot be empty.", nameof(value));
            if (value.Length > 200)
                throw new ArgumentException("Matn title cannot exceed 200 characters.", nameof(value));
            _title = value;
        }
    }

    public string? Author { get; private set; }
    public string? Category { get; private set; }
    public string? Description { get; private set; }
    public MatnStatus Status { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyList<MatnSection> Sections => _sections.AsReadOnly();

    // EF Core constructor
    private Matn() { }

    public static Matn Create(string title, string? author = null, string? category = null, string? description = null)
    {
        return new Matn
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author,
            Category = category,
            Description = description,
            Status = MatnStatus.Draft,
            IsDeleted = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(string title, string? author, string? category, string? description)
    {
        Title = title;
        Author = author;
        Category = category;
        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Activate()
    {
        Status = MatnStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        Status = MatnStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Status = MatnStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public MatnSection AddSection(string title)
    {
        var nextOrder = _sections.Count == 0 ? 1 : _sections.Max(s => s.Order) + 1;
        var section = MatnSection.Create(Id, title, nextOrder);
        _sections.Add(section);
        UpdatedAt = DateTimeOffset.UtcNow;
        return section;
    }
}
