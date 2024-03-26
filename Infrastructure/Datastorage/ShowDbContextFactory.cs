using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Showtime.Infrastructure.Datastorage;
public class ShowDbContextFactory : IDesignTimeDbContextFactory<ShowDbContext>
{
    public ShowDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShowDbContext>();
        optionsBuilder.UseSqlServer();

        return new ShowDbContext(optionsBuilder.Options);
    }
}
