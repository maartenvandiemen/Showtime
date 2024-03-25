using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Showtime.Infrastructure.Datastorage;
using Testing;

namespace Showtime.Infrastructure.UnitTests;
[TestClass]
public class ShowtimeRepositoryTests
{
    private Fixture _fixture = CustomFixture.Create();
    private ShowDbContext _context = null!;
    private ShowtimeRepository _sut = null!;
    private ILogger<ShowtimeRepository> _logger = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ShowDbContext>()
           .UseInMemoryDatabase("ShowtimeDB")
           .Options;

        _context = new ShowDbContext(options);
        _logger = new Mock<ILogger<ShowtimeRepository>>().Object;

        _sut = new ShowtimeRepository(_context, _logger);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _context?.Database?.EnsureDeleted();
        _context?.Dispose();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullDbContext_ArgumentNullExceptionThrown()
    {
        //Arrange
        _context = null!;

        //Act
        _ = new ShowtimeRepository(_context, _logger);

        //Assert
        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_NullLogger_ArgumentNullExceptionThrown()
    {
        //Arrange
        _logger = null!;

        //Act
        _ = new ShowtimeRepository(_context, _logger);

        //Assert
        //Expected exception
    }

    [TestMethod]
    public async Task AddPage_NoSyncStatus_AddPerformed()
    {
        //Arrange
        int pagenumber = _fixture.Create<int>();

        //Act
        await _sut.AddPage(pagenumber);
        await _context.CommitAsync();

        //Assert
        Assert.AreEqual(1, _context.SyncStatus.Count());
    }

    [TestMethod]
    public async Task AddPage_SyncStatusAvailable_UpdatePerformedDateProcessedUpdated()
    {
        //Arrange
        int pagenumber = _fixture.Create<int>();

        var initialSyncStatus = new SyncStatus(pagenumber);
        var initialDate = initialSyncStatus.DateProcessed;

        _context.SyncStatus.Add(initialSyncStatus);
        await _context.SaveChangesAsync();

        //Act
        await _sut.AddPage(pagenumber);
        await _context.CommitAsync();

        //Assert
        Assert.AreEqual(1, _context.SyncStatus.Count());

        var updatedSyncStatus = _context.SyncStatus.FirstOrDefault();
        Assert.IsTrue(initialDate < updatedSyncStatus!.DateProcessed);
    }

    [TestMethod]
    public async Task GetLastStoredPage_NoSyncStatus_0Returned()
    {
        //Arrange & Act
        var result = await _sut.GetLastStoredPage();

        //Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task GetLastStoredPage_SyncStatus_Returned()
    {
        //Arrange
        int pagenumber = _fixture.Create<int>();

        _context.SyncStatus.Add(new SyncStatus(pagenumber));
        _context.SyncStatus.Add(new SyncStatus(++pagenumber));

        await _context.SaveChangesAsync();

        //Act
        var result = await _sut.GetLastStoredPage();

        //Assert
        Assert.AreEqual(pagenumber, result);
    }

    [TestMethod]
    public async Task ShowExists_NoMatchingShows_FalseReturned()
    {
        //Arrange
        int showId = await AddRandomShow();

        //Act
        var result = await _sut.ShowExistsAsync(--showId);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task ShowExists_MathingShows_TrueReturned()
    {
        //Arrange
        int showId = await AddRandomShow();

        //Act
        var result = await _sut.ShowExistsAsync(showId);

        //Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task AddShow_NewShow_ShowAdded()
    {
        //Arrange
        var show = GenerateRandomShow();

        //Act
        await _sut.AddShow(show);
        await _context.CommitAsync();

        //Assert
        Assert.AreEqual(1, _context.Shows.Count());
    }

    [TestMethod]
    public async Task AddShow_ExistingShow_NoAdditionalShowAdded()
    {
        //Arrange
        var existingShowId = await AddRandomShow();
        var showToAdd = GenerateRandomShow(existingShowId);

        //Act
        await _sut.AddShow(showToAdd);
        await _context.CommitAsync();

        //Assert
        Assert.AreEqual(1, _context.Shows.Count());
    }

    private async Task<int> AddRandomShow()
    {
        var show = GenerateRandomShow();

        _context.Shows.Add(show);
        await _context.SaveChangesAsync();

        return show.Id;
    }

    private Show GenerateRandomShow(int? id = null)
    {
        return new Show
        {
            Id = id == null ? _fixture.Create<int>() : id.GetValueOrDefault(),
            Name = _fixture.Create<string>(),
            Language = _fixture.Create<string>(),
            Summary = _fixture.Create<string>()
        };
    }
}
