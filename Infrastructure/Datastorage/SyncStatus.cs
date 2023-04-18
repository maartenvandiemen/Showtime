namespace Showtime.Infrastructure.Datastorage;

internal class SyncStatus
{
    public SyncStatus(int pagenumber)
    {
        if(pagenumber < 0) { throw new ArgumentNullException(nameof(pagenumber)); }

        Pagenumber = pagenumber;
        DateProcessed = DateTime.Now;
    }

    public DateTime DateProcessed { get; private set; }
    public int Pagenumber { get; set; }

    public void ResetDateProcessed()
    {
        DateProcessed = DateTime.Now;
    }
}
