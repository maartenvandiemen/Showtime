using AutoFixture;
using Showtime.ShowtimeApi.Models;
using Testing;

namespace ShowTimeApi.UnitTests;

[TestClass]
public class FindShowsResponseTests
{
    [TestMethod]
    public void FindShowsResponse_Map_MappingCorrect()
    {
        //Arrange
        Fixture fixture = CustomFixture.Create();
        var input = fixture.CreateMany<Showtime.Domain.Show>(2);

        //Act
        var result = FindShowsResponse.Map(input);

        //Assert
        Assert.AreEqual(2, result.Shows.Count());
        var firstInputItem = input.FirstOrDefault();
        var firstMappedItem = result.Shows.FirstOrDefault();

        AssertShow(firstInputItem!, firstMappedItem!);

        var lastInputItem = input.LastOrDefault();
        var lastMappedItem = result.Shows.LastOrDefault();

        AssertShow(lastInputItem!, lastMappedItem!);
    }

    private void AssertShow(Showtime.Domain.Show input, Showtime.ShowtimeApi.Models.Show output)
    {
        Assert.AreEqual(input.Id, output.Id);
        Assert.AreEqual(input.Name, output.Name);
        Assert.AreEqual(input.Language, output.Language);
        Assert.AreEqual(input.Genres, output.Genres);
        Assert.AreEqual(input.Premiered, output.Premiered);
    }
}