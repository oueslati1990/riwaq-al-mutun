namespace MatnMemorization.Domain.Entities;

public class MatnSection
{
    private readonly List<TextUnit> _textUnits = [];
    private string _title = default!;

    public Guid Id { get; private set; }
    public Guid MatnId { get; private set; }

    public string Title
    {
        get => _title;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Section title cannot be empty.", nameof(value));
            if (value.Length > 200)
                throw new ArgumentException("Section title cannot exceed 200 characters.", nameof(value));
            _title = value;
        }
    }

    public int Order { get; private set; }
    public IReadOnlyList<TextUnit> TextUnits => _textUnits.AsReadOnly();

    // EF Core constructor
    private MatnSection() { }

    public static MatnSection Create(Guid matnId, string title, int order)
    {
        return new MatnSection
        {
            Id = Guid.NewGuid(),
            MatnId = matnId,
            Title = title,
            Order = order
        };
    }

    public void UpdateTitle(string title) => Title = title;

    public void UpdateOrder(int order) => Order = order;

    public TextUnit AddTextUnit(string content)
    {
        var nextOrder = _textUnits.Count == 0 ? 1 : _textUnits.Max(u => u.Order) + 1;
        var unit = TextUnit.Create(Id, content, nextOrder);
        _textUnits.Add(unit);
        return unit;
    }
}
