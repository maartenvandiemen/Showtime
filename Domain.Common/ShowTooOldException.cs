using System.Runtime.Serialization;

namespace Showtime.Domain;
public class ShowTooOldException : Exception
{
    public ShowTooOldException()
    {
    }

    public ShowTooOldException(string? message) : base(message)
    {
    }

    public ShowTooOldException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ShowTooOldException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
