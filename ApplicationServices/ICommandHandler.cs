using Showtime.Domain;

namespace Showtime.ApplicationServices;

public interface ICommandHandler<TCommand> where TCommand : Command
{
    Task HandleAsync(TCommand command);
}
