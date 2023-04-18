using Microsoft.Extensions.Logging;
using Showtime.Domain;

namespace Showtime.ApplicationServices;
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
            try
            {
                var showToAdd = Show.Register(command);

                _repository.AddShow(showToAdd);
            }
            catch (ShowTooOldException)
            {
                _logger.LogInformation($"Show with ID {command.Id} skipped. Premiered date: {command.Premiered}");
            }
        }
        else
        {
            _logger.LogDebug($"Show with ID {command.Id} already exists. Skipping...");
        }
    }
}
