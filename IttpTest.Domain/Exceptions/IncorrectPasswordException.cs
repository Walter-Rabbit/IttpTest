namespace IttpTest.Domain.Exceptions;

public class IncorrectPasswordException : Exception
{
    public IncorrectPasswordException()
    {
    }

    public IncorrectPasswordException(string message)
        : base(message)
    {
    }

    public IncorrectPasswordException(string message, Exception e)
        : base(message, e)
    {
    }
}