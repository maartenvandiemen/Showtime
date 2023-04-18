namespace Showtime.Infrastructure.Datastorage;
public interface ISyncStatusRepository
{
    public Task AddPage(int pagenumber);

    public Task<int> GetLastStoredPage();
}
