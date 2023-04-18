using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Showtime.ApplicationServices;
using Showtime.Domain;
using Showtime.Infrastructure;
using Showtime.Infrastructure.Datastorage;
using Showtime.Infrastructure.ExternalRepository;
using Showtime.WorkerService;
using Testing;

namespace WorkerService.UnitTests;

[TestClass]
public class ShowScraperServiceTests
{
    private Fixture _fixture = CustomFixture.Create();
    private ShowScraperService _sut = null!;
    private Mock<ILogger<ShowScraperService>> _loggerMock = null!;
    private Mock<ISyncStatusRepository> _syncStatusRepositoryMock = null!;
    private Mock<ITvMazeRepository> _tvMazeRepositoryMock = null!;
    private Mock<ICommandHandler<RegisterShow>> _commandHanderMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _loggerMock = new Mock<ILogger<ShowScraperService>>();
        _syncStatusRepositoryMock = new Mock<ISyncStatusRepository>();
        _tvMazeRepositoryMock = new Mock<ITvMazeRepository>();
        _commandHanderMock = new Mock<ICommandHandler<RegisterShow>>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _syncStatusRepositoryMock.SetupSequence(method => method.GetLastStoredPage())
            //First since no record can be found
            .ReturnsAsync(0)
            //Second time due to last processed record
            .ReturnsAsync(0);

        _sut = new ShowScraperService(_loggerMock.Object, _syncStatusRepositoryMock.Object, _tvMazeRepositoryMock.Object,
            _commandHanderMock.Object, _unitOfWorkMock.Object);
    }

    #region constructor
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_LoggerNull_ArgumentNullExceptionThrown()
    {
        //Arrange & Act
        _ = new ShowScraperService(null!, _syncStatusRepositoryMock.Object, _tvMazeRepositoryMock.Object,
            _commandHanderMock.Object, _unitOfWorkMock.Object);

        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_SyncStatusRepositoryNull_ArgumentNullExceptionThrown()
    {
        //Arrange & Act
        _ = new ShowScraperService(_loggerMock.Object, null!, _tvMazeRepositoryMock.Object,
            _commandHanderMock.Object, _unitOfWorkMock.Object);

        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_TvMazeRepositoryNull_ArgumentNullExceptionThrown()
    {
        //Arrange & Act
        _ = new ShowScraperService(_loggerMock.Object, _syncStatusRepositoryMock.Object, null!,
            _commandHanderMock.Object, _unitOfWorkMock.Object);

        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_CommandHandlerNull_ArgumentNullExceptionThrown()
    {
        //Arrange & Act
        _ = new ShowScraperService(_loggerMock.Object, _syncStatusRepositoryMock.Object, _tvMazeRepositoryMock.Object,
            null!, _unitOfWorkMock.Object);

        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_UnitOfWorkNull_ArgumentNullExceptionThrown()
    {
        //Arrange & Act
        _ = new ShowScraperService(_loggerMock.Object, _syncStatusRepositoryMock.Object, _tvMazeRepositoryMock.Object,
            _commandHanderMock.Object, null!);

        //Expected exception
    }
    #endregion

    [TestMethod]
    public async Task LoadShows_PagenumberFirstIteration_CorrectPagenumberUsed()
    {
        //Arrange
        _syncStatusRepositoryMock.Setup(method => method.GetLastStoredPage()).ReturnsAsync(0);
        _tvMazeRepositoryMock.Setup(method => method.LoadShowsFromApiByPagenumber(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ShowFromApi>());

        //Act
        await _sut.LoadShowsAsync(new CancellationToken());

        //Assert
        _tvMazeRepositoryMock.Verify(method => method.LoadShowsFromApiByPagenumber(0, It.IsAny<CancellationToken>()), Times.Once());

        _unitOfWorkMock.Verify(method => method.CommitAsync(), Times.Once());
    }

    [TestMethod]
    public async Task LoadShows_PagenumberSecondIteration_CorrectPagenumberUsed()
    {
        //Arrange
        _tvMazeRepositoryMock.SetupSequence(method => method.LoadShowsFromApiByPagenumber(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShowFromApi>() { _fixture.Create<ShowFromApi>() })
            .ReturnsAsync(new List<ShowFromApi>());

        //Act
        await _sut.LoadShowsAsync(new CancellationToken());

        //Assert
        _tvMazeRepositoryMock.Verify(method => method.LoadShowsFromApiByPagenumber(0, It.IsAny<CancellationToken>()), Times.Once());
        _tvMazeRepositoryMock.Verify(method => method.LoadShowsFromApiByPagenumber(1, It.IsAny<CancellationToken>()), Times.Once());
        
        _syncStatusRepositoryMock.Verify(method => method.AddPage(0), Times.Once());
        //Since the second time no results are returned, so the page won't be stored
        _syncStatusRepositoryMock.Verify(method => method.AddPage(1), Times.Never());
        
        _unitOfWorkMock.Verify(method => method.CommitAsync(), Times.Exactly(2));
    }

    [TestMethod]
    public async Task LoadShows_CommandHandlerCalled_SameAmountAsRetrievedTvShows()
    {
        //Arrange
        _tvMazeRepositoryMock.SetupSequence(method => method.LoadShowsFromApiByPagenumber(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ShowFromApi>(_fixture.CreateMany<ShowFromApi>(240)))
            .ReturnsAsync(new List<ShowFromApi>());

        //Act
        await _sut.LoadShowsAsync(new CancellationToken());

        //Assert
        _commandHanderMock.Verify(method => method.HandleAsync(It.IsAny<RegisterShow>()), Times.Exactly(240));
    }
}