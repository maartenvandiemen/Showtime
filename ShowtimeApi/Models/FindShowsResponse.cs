namespace Showtime.ShowtimeApi.Models;

public class FindShowsResponse
{
    public FindShowsResponse()
    {
        Shows = new List<Show>();
    }

    public IEnumerable<Show> Shows { get; set; }

    public static FindShowsResponse Map(IEnumerable<Domain.Show> domainShows)
    {
        return new FindShowsResponse
        {
            Shows = domainShows.Select(s => new Show
            {
                Language = s.Language,
                Name = s.Name,
                Summary = s.Summary,
                Genres = s.Genres,
                Id = s.Id,
                Premiered = s.Premiered
            }),
        };
    }
}
