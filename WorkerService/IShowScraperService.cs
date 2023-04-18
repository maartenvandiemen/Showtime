namespace Showtime.WorkerService;
internal interface IShowScraperService
{
    Task LoadShowsAsync(CancellationToken cancellationToken);
}
