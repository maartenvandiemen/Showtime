using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Showtime.Infrastructure.Datastorage;
public class ShowtimeRepository : ISyncStatusRepository, IShowRepository
{
    private readonly ShowDbContext _dbContext;
    private readonly ILogger<ShowtimeRepository> _logger;

    public ShowtimeRepository(ShowDbContext dbContext, ILogger<ShowtimeRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddPage(int pagenumber)
    {
        var syncStatus = await _dbContext.SyncStatus.FirstOrDefaultAsync(s => s.Pagenumber == pagenumber);

        if (syncStatus is not null)
        {
            syncStatus.ResetDateProcessed();
            _dbContext.SyncStatus.Update(syncStatus);
        }
        else
        {
            var syncStatusToAdd = new SyncStatus(pagenumber);
            _dbContext.SyncStatus.Add(syncStatusToAdd);
        }
    }

    public async Task AddShow(Show show)
    {
        if (!await ShowExistsAsync(show.Id))
        {
            _dbContext.Shows.Add(show);
        }
        else
        {
            _logger.LogDebug("Show with ID {showId} already exists. Skipping...", show.Id);
        }        
    }

    public async Task<int> GetLastStoredPage()
    {
        var lastPage = await _dbContext.SyncStatus.OrderByDescending(p => p.Pagenumber).FirstOrDefaultAsync();
        return lastPage != null ? lastPage.Pagenumber : 0;
    }

    public Task<bool> ShowExistsAsync(int id)
    {
        return _dbContext.Shows.AnyAsync(s => s.Id == id);
    }
}
