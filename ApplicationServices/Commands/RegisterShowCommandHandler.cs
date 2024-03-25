using Microsoft.Extensions.Logging;
using Showtime.ApplicationServices;
using Showtime.Core.Domain;

namespace Showtime.Core.Commands;
internal class RegisterShowCommandHandler : ICommandHandler<RegisterShow>
{
    private readonly IShowRepository _repository;
    private readonly ILogger _logger;

    public RegisterShowCommandHandler(ILogger<RegisterShowCommandHandler> logger, IShowRepository showRepository)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(showRepository, nameof(showRepository));

        _repository = showRepository;
        _logger = logger;
    }

    public async Task HandleAsync(RegisterShow command)
    {
        if (!await _repository.ShowExistsAsync(command.Id))
        {
            var showToAdd = Show.Register(command);

            _repository.AddShow(showToAdd);
        }
        else
        {
            _logger.LogDebug($"Show with ID {command.Id} already exists. Skipping...");
        }
    }
}
