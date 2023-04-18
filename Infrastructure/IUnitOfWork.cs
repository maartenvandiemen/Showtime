namespace Showtime.Infrastructure;
public interface IUnitOfWork
{
    Task CommitAsync();
}
