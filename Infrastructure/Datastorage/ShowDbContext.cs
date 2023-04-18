using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Showtime.Domain;
using Showtime.Infrastructure.Datastorage.TypeConfigurations;

namespace Showtime.Infrastructure.Datastorage;
public class ShowDbContext : DbContext, IUnitOfWork
{
    private const int SqlErrorKeyConstraintViolation = 2627;

    public ShowDbContext(DbContextOptions<ShowDbContext> options) : base(options) { }

    internal DbSet<SyncStatus> SyncStatus { get; set; }

    internal DbSet<Show> Shows { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SyncStatusTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ShowTypeConfiguration());
    }

    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
        }
        catch (DbUpdateException exception) when
            (exception.InnerException is SqlException { Number: SqlErrorKeyConstraintViolation })
        {
            throw new ConcurrencyException("Version conflict.", exception);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyException("Version conflict.", exception);
        }
    }
}
