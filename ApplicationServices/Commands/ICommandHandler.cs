namespace Showtime.Core.Commands;

public interface ICommandHandler<TCommand> where TCommand : Command
{
    Task HandleAsync(TCommand command);
}
