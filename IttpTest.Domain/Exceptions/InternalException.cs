namespace IttpTest.Domain.Exceptions;

public class InternalException : Exception
{
    public InternalException()
    {
    }

    public InternalException(string message)
        : base(message)
    {
    }

    public InternalException(string message, Exception e)
        : base(message, e)
    {
    }
}