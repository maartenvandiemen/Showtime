namespace Showtime.Infrastructure.Datastorage;

public interface IShowRepository
{
    public Task<bool> ShowExistsAsync(int id);

    public Task AddShow(Show show);
}
