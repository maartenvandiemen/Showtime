using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Showtime.Infrastructure.Datastorage;

/// <summary>
/// This class is only used when EF Migrations is used. Not used in scenario when the Worker is running.
/// </summary>
public class ShowDbContextFactory : IDesignTimeDbContextFactory<ShowDbContext>
{
    public ShowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShowDbContext>();
        optionsBuilder.UseSqlServer();

        return new ShowDbContext(optionsBuilder.Options);
    }
}
