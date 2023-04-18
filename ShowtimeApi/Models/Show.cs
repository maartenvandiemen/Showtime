namespace Showtime.ShowtimeApi.Models;

/// <summary>
/// Show retrieved from the TvMaze API
/// </summary>
public class Show
{
    public Show()
    {
        Genres = new List<string>();
    }

    /// <summary>
    /// Id from the TvMaze API
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Name of the show
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Language of the show
    /// </summary>
    public required string Language { get; set; }
    /// <summary>
    /// Date when the show premiered
    /// </summary>
    public DateOnly? Premiered { get; set; }
    /// <summary>
    /// Genres of the show
    /// </summary>
    public IReadOnlyCollection<string> Genres { get; set; }
    /// <summary>
    /// Summary of the show
    /// </summary>
    public required string? Summary { get; set; }
}
