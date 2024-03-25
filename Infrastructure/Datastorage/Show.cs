namespace Showtime.Infrastructure.Datastorage;
public record Show
{
    public Show()
    {
        Genres = new List<string>().AsReadOnly();
    }

    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Language { get; init; }
    public DateOnly? Premiered { get; init; }
    public IReadOnlyCollection<string> Genres { get; init; }
    public required string? Summary { get; init; }
}
