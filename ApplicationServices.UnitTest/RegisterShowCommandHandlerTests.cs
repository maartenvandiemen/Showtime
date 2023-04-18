using AutoFixture;
using Showtime.Domain.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Showtime.ApplicationServices;
using Showtime.Domain;
using Testing;

namespace ApplicationServices.UnitTest;

[TestClass]
public class RegisterShowCommandHandlerTests
{
    private Fixture _fixture = CustomFixture.Create();
    private Mock<IShowRepository> _repositoryMock = null!;
    private Mock<ILogger<RegisterShowCommandHandler>> _loggerMock = null!;
    private RegisterShowCommandHandler _sut = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _repositoryMock = new Mock<IShowRepository>();
        _loggerMock = new Mock<ILogger<RegisterShowCommandHandler>>();

        _sut = new RegisterShowCommandHandler(_loggerMock.Object, _repositoryMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ArgumentNullLogger_ArgumentNullExceptionThrown()
    {
        //Arrange
        ILogger<RegisterShowCommandHandler> logger = null!;
        IShowRepository showRepository = new Mock<IShowRepository>().Object;

        //Act
        new RegisterShowCommandHandler(logger, showRepository);

        //Assert
        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ArgumentNullRepository_ArgumentNullExceptionThrown()
    {
        //Arrange
        ILogger<RegisterShowCommandHandler> logger = new Mock<ILogger<RegisterShowCommandHandler>>().Object;
        IShowRepository showRepository = null!;

        //Act
        new RegisterShowCommandHandler(logger, showRepository);

        //Assert
        //Expected exception
    }

    [TestMethod]
    public async Task HandleAsync_ShowExists_NothingAdded()
    {
        //Arrange
        _repositoryMock.Setup(method => method.ShowExistsAsync(It.IsAny<int>())).ReturnsAsync(true);

        //Act
        await _sut.HandleAsync(_fixture.Create<RegisterShow>());

        //Assert
        _repositoryMock.Verify(method => method.AddShow(It.IsAny<Show>()), Times.Never());
    }

    [TestMethod]
    public async Task HandleAsync_ShowTooOldException_NothingAdded()
    {
        //Arrange
        _repositoryMock.Setup(method => method.ShowExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        //Act
        var command = new RegisterShowBuilder().Build() with { Premiered = new DateOnly(2013, 1, 1) };

        await _sut.HandleAsync(command);

        //Assert
        _repositoryMock.Verify(method => method.AddShow(It.IsAny<Show>()), Times.Never());
    }

    [TestMethod]
    public async Task HandleAsync_Correct_ShowAdded()
    {
        //Arrange
        _repositoryMock.Setup(method => method.ShowExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        //Act
        var command = new RegisterShowBuilder().Build();

        await _sut.HandleAsync(command);

        //Assert
        _repositoryMock.Verify(method => method.AddShow(It.IsAny<Show>()), Times.Once());
    }
}