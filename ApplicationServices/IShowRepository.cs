using Showtime.Core.Domain;

namespace Showtime.ApplicationServices;

public interface IShowRepository
{
    public Task<bool> ShowExistsAsync(int id);

    public void AddShow(Show show);
}
