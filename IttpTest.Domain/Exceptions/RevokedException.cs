namespace IttpTest.Domain.Exceptions;

public class RevokedException : Exception
{
    public RevokedException()
    {
    }

    public RevokedException(string message)
        : base(message)
    {
    }

    public RevokedException(string message, Exception e)
        : base(message, e)
    {
    }
}