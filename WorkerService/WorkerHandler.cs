using Microsoft.Azure.WebJobs;
using Showtime.WorkerService;

namespace WorkerService;

public class WorkerHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WorkerHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory; 
    }

    [NoAutomaticTrigger]
    public async Task ExecuteAsync(
        ILogger logger, 
        CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var showScraperService = scope.ServiceProvider.GetRequiredService<IShowScraperService>();

                await showScraperService.LoadShowsAsync(cancellationToken);
            }
            int minutes = 5;
            logger.LogInformation("Worker waiting for {numberOfMinutes} minutes at: {time}", minutes, DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromMinutes(minutes), cancellationToken);
        }
    }
}
