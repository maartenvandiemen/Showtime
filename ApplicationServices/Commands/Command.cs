namespace Showtime.Core.Commands;

public record Command
{
    public Guid CommandId { get; } = Guid.NewGuid();
}