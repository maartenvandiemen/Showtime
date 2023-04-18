namespace Showtime.Infrastructure.ExternalRepository;
public interface ITvMazeRepository
{
    Task<IEnumerable<ShowFromApi>> LoadShowsFromApiByPagenumber(int showPageNumber, CancellationToken cancellationToken);
}
