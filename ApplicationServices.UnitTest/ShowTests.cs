using Showtime.Core.Commands;
using Showtime.Core.Domain;
using Showtime.Domain.Testing;

namespace Showtime.Domain.UnitTests;

[TestClass]
public class ShowTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Register_NullCommand_ArgumentNullExceptionThrown()
    {
        //Arrange
        RegisterShow command = null!;

        //Act
        Show.Register(command);

        //Assert
        //Expected exception
    }

    [TestMethod]
    [ExpectedException(typeof(ShowTooOldException))]
    public void Register_CommandShowTooOld_ShowTooOldExceptionThrown()
    {
        //Arrange
        RegisterShow command = new RegisterShowBuilder().Build() with { Premiered = new DateOnly(2013,12,31) };

        //Act
        Show.Register(command);

        //Assert
        //Expected exception
    }

    [TestMethod]
    public void Register_Command_CorrectShow()
    {
        //Arrange
        RegisterShow command = new RegisterShowBuilder().Build() with { Premiered = new DateOnly(2014, 1, 1) };

        //Act
        var show = Show.Register(command);

        //Assert
        Assert.AreEqual(command.Language, show.Language);
        Assert.AreEqual(command.Name, show.Name);
        Assert.AreEqual(command.Summary, show.Summary);
        Assert.AreEqual(command.Genres, show.Genres);
        Assert.AreEqual(command.Premiered, show.Premiered);
        Assert.AreEqual(command.Id, show.Id);
    }
}
