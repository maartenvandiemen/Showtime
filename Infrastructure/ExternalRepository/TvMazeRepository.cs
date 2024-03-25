using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Showtime.Infrastructure.ExternalRepository;
public class TvMazeRepository : ITvMazeRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TvMazeRepository> _logger;

    public TvMazeRepository(ILogger<TvMazeRepository> logger, HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ShowFromApi>> LoadShowsFromApiByPagenumber(int showPageNumber, CancellationToken cancellationToken)
    {
        List<ShowFromApi> showsFromApi = new();
        try
        {
            _logger.LogDebug("Getting shows for page: {showPageNumber}", showPageNumber);

            var showsRetrievedFromApi = await _httpClient.GetFromJsonAsync<IEnumerable<ShowFromApi>>($"shows?page={showPageNumber}", cancellationToken);

            if (showsRetrievedFromApi != null)
            {
                showsFromApi.AddRange(showsRetrievedFromApi);
            }
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError("{showPageNumber} NOT FOUND.", showPageNumber);
                return Enumerable.Empty<ShowFromApi>();
            }

            _logger.LogError("HTTP error occurred. Status code: {statusCode} - error: {message}", e.StatusCode, e.Message);
            throw;
        }

        return showsFromApi;
    }
}
