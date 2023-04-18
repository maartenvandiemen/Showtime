namespace Showtime.Domain;
public record Command
{
    public Guid CommandId { get; } = Guid.NewGuid();
}