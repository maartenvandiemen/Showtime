using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Showtime.Infrastructure.ExternalRepository;
using Showtime.Infrastructure.UnitTests;

namespace Infrastructure.UnitTests;

[TestClass]
public class TvMazeRepositoryTests
{
    private Mock<ILogger<TvMazeRepository>> _loggerMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<TvMazeRepository>>();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_LoggerIsNull_ArgumentNullExceptionThrown()
    {
        //Arrange
        ILogger<TvMazeRepository> logger = null!;
        var httpClient = SetupHttpClient(HttpStatusCode.OK, Resource.Page0Response);

        //Act
        new TvMazeRepository(logger, httpClient);

        //Assert
        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_HttpClientIsNull_ArgumentNullExceptionThrown()
    {
        //Arrange
        HttpClient httpClient = null!;

        //Act
        new TvMazeRepository(_loggerMock.Object, httpClient);

        //Assert
        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(HttpRequestException))]
    public async Task LoadShowsFromApiByPagenumber_NegativePagenumber_ExceptionThrown()
    {
        //Arrange
        var httpClient = SetupHttpClient(HttpStatusCode.BadRequest, Resource.BadRequest);
        var cancellationToken = new CancellationToken();

        var sut = new TvMazeRepository(_loggerMock.Object, httpClient);

        //Act
        await sut.LoadShowsFromApiByPagenumber(-1, cancellationToken);

        //Assert
        //Expected exception
    }

    [TestMethod]
    public async Task LoadShowsFromApiByPagenumber_NotFound_ExceptionThrown()
    {
        //Arrange
        var httpClient = SetupHttpClient(HttpStatusCode.NotFound, Resource.EmptyResponse);
        var cancellationToken = new CancellationToken();

        var sut = new TvMazeRepository(_loggerMock.Object, httpClient);

        //Act
        var result = await sut.LoadShowsFromApiByPagenumber(12345, cancellationToken);

        //Assert
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task LoadShowsFromApiByPagenumber_Found_CorrectResult()
    {
        //Arrange
        var httpClient = SetupHttpClient(HttpStatusCode.OK, Resource.Page0Response);
        var cancellationToken = new CancellationToken();

        var sut = new TvMazeRepository(_loggerMock.Object, httpClient);

        //Act
        var results = await sut.LoadShowsFromApiByPagenumber(1, cancellationToken);

        //Assert
        Assert.AreEqual(240, results.Count());

        var showFromApi = results.FirstOrDefault();

        Assert.IsNotNull(showFromApi);
        Assert.AreEqual("Under the Dome", showFromApi.Name);
        Assert.AreEqual(338, showFromApi.Summary!.Length);
        Assert.AreEqual("English", showFromApi.Language);
        Assert.AreEqual(new DateOnly(2013, 6, 24), showFromApi.Premiered);
        Assert.AreEqual(1, showFromApi.Id);
        Assert.AreEqual(3, showFromApi.Genres.Count);
    }

    private HttpClient SetupHttpClient(HttpStatusCode statuscode, string responseJson)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = statuscode,
            Content = new StringContent(responseJson),
        };

        handlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
          .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://test.com/")
        };
    }
}