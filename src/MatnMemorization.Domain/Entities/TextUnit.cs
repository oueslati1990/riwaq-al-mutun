using MatnMemorization.Domain.Utilities;

namespace MatnMemorization.Domain.Entities;

public class TextUnit
{
    private string _content = default!;

    public Guid Id { get; private set; }
    public Guid SectionId { get; private set; }

    public string Content
    {
        get => _content;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("TextUnit content cannot be empty.", nameof(value));
            if (value.Length > 2000)
                throw new ArgumentException("TextUnit content cannot exceed 2000 characters.", nameof(value));
            _content = value;
            SearchIndex = ArabicNormalizer.ToSearchIndex(value);
        }
    }

    public string SearchIndex { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    // EF Core constructor
    private TextUnit() { }

    public static TextUnit Create(Guid sectionId, string content, int order)
    {
        return new TextUnit
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Content = content,
            Order = order,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateContent(string content)
    {
        Content = content;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
