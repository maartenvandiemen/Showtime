using Showtime.Core.Commands;

namespace Showtime.Core.Domain;

public class Show
{
    public Show()
    {
        Genres = new List<string>().AsReadOnly();
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Language { get; set; }
    public DateOnly? Premiered { get; set; }
    public IReadOnlyCollection<string> Genres { get; set; }
    public required string? Summary { get; set; }

    public static Show Register(RegisterShow command)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));

        return new Show()
        {
            Id = command.Id,
            Name = command.Name,
            Language = command.Language,
            Premiered = command.Premiered,
            Genres = command.Genres,
            Summary = command.Summary
        };
    }
}
