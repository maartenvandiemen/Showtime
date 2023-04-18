using Microsoft.AspNetCore.Mvc;
using Showtime.ApplicationServices;
using Showtime.ShowtimeApi.Models;

namespace Showtime.ShowtimeApi.Controllers;

[Route("shows")]
[ApiController]
public class ShowsController : Controller
{
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(FindShowsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleQueryFindTvShows([FromServices]IShowRepository repo, [FromQuery]int lastId)
    {
        ArgumentNullException.ThrowIfNull(repo, nameof(repo));

        if(lastId < 0)
        {
            return BadRequest();
        }

        var shows = await repo.FindAllShows(lastId);

        return Ok(FindShowsResponse.Map(shows));
    }
}
