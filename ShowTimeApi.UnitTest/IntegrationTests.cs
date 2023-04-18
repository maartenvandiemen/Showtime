using System.Net.Http.Json;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Showtime.Infrastructure.Datastorage;
using Showtime.ShowtimeApi.Models;
using Testing;

namespace Showtime.ShowtimeApi.UnitTests;
[TestClass]
public class IntegrationTests
{
    [TestMethod]
    public async Task FindAllShows_EmptyDatabase_NoResults()
    {
        //Arrange
        await using var application = new ShowtimeApiApplication();
        var client = application.CreateClient();

        //Act
        var result = await client.GetFromJsonAsync<FindShowsResponse>("shows?lastId=1");

        //Assert
        Assert.AreEqual(0, result!.Shows.Count());
    }

    [TestMethod]
    public async Task FindAllShows_NoEmptyDatabase_Results()
    {
        //Arrange
        Fixture fixture = CustomFixture.Create();
        await using var application = new ShowtimeApiApplication();
        using (var scope = application.Services.CreateScope())
        {
            var provider = scope.ServiceProvider;
            using var showtimeDbContext = provider.GetRequiredService<ShowDbContext>();
            await showtimeDbContext.Database.EnsureCreatedAsync();

            foreach (var show in fixture.CreateMany<Domain.Show>(100))
            {
                showtimeDbContext.Shows.Add(show);
            }
            await showtimeDbContext.SaveChangesAsync();
        }
        var client = application.CreateClient();

        //Act
        var result = await client.GetFromJsonAsync<FindShowsResponse>("shows?lastId=0");

        //Assert
        Assert.AreEqual(100, result!.Shows.Count());
    }
}
