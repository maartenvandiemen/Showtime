using Showtime.Infrastructure.Datastorage;

namespace Showtime.Infrastructure.ExternalRepository;
public record ShowFromApi
{
    public ShowFromApi()
    {
        Genres = new List<string>().AsReadOnly();
    }

    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Language { get; init; }
    public DateOnly? Premiered { get; init; }
    public IReadOnlyCollection<string> Genres { get; init; }
    public required string? Summary { get; init; }

    public Show ToDatabase()
    {
        return new Show() 
        {
            Id = Id,
            Name = Name,
            Language = Language, 
            Premiered = Premiered,
            Genres = Genres,
            Summary = Summary 
        };       
    }
}
