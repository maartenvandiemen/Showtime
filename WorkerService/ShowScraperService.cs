using Showtime.Infrastructure;
using Showtime.Infrastructure.Datastorage;
using Showtime.Infrastructure.ExternalRepository;

namespace Showtime.WorkerService;
internal class ShowScraperService : IShowScraperService
{
    private readonly ILogger<ShowScraperService> _logger;
    private readonly ISyncStatusRepository _syncStatusRepository;
    private readonly ITvMazeRepository _tvMazeRepository;
    private readonly IShowRepository _showRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShowScraperService(ILogger<ShowScraperService> logger, 
        ISyncStatusRepository syncStatusRepository, 
        ITvMazeRepository tvMazeRepository,
        IShowRepository showRepository,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(syncStatusRepository, nameof(syncStatusRepository));
        ArgumentNullException.ThrowIfNull(tvMazeRepository, nameof(tvMazeRepository));
        ArgumentNullException.ThrowIfNull(showRepository, nameof(showRepository));
        ArgumentNullException.ThrowIfNull(unitOfWork, nameof(unitOfWork));

        _logger = logger;
        _syncStatusRepository = syncStatusRepository;
        _tvMazeRepository = tvMazeRepository;
        _showRepository = showRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task LoadShowsAsync(CancellationToken cancellationToken)
    {
        var firstIteration = true;
        var continueSearch = true;

        while(continueSearch && !cancellationToken.IsCancellationRequested)
        {
            var pagenumber = await _syncStatusRepository.GetLastStoredPage();

            pagenumber = firstIteration ? pagenumber : ++pagenumber;

            continueSearch = await LoadShowsByPageNumber(pagenumber, cancellationToken);

            await _unitOfWork.CommitAsync();
            firstIteration = false;
            if (!continueSearch)
            {
                _logger.LogInformation("Show page number {pagenumber} NOT found. Stopping search.", pagenumber);
                break;
            }
        }
    }

    internal async Task<bool> LoadShowsByPageNumber(int pagenumber, CancellationToken cancellationToken)
    {
        var showsFromApi = await _tvMazeRepository.LoadShowsFromApiByPagenumber(pagenumber, cancellationToken);

        if (!showsFromApi.Any())
        {
            _logger.LogDebug("Pagenumber {pagenumber} did not return any results.", pagenumber);
            return false;
        }

        foreach (var showFromApi in showsFromApi)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Cancellation requested. Stopping...");
                break;
            }

            _logger.LogDebug("Adding show ID: {showId} - Name: {showName}", showFromApi.Id, showFromApi.Name);
            await _showRepository.AddShow(showFromApi.ToDatabase());
        }

        _logger.LogDebug("ShowDB: Adding show page number: {pagenumber}", pagenumber);
        await _syncStatusRepository.AddPage(pagenumber);
        return true;
    }
}
