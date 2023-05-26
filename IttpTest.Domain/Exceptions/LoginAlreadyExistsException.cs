namespace IttpTest.Domain.Exceptions;

public class LoginAlreadyExistsException : Exception
{
    public LoginAlreadyExistsException()
    {
    }

    public LoginAlreadyExistsException(string message)
        : base(message)
    {
    }

    public LoginAlreadyExistsException(string message, Exception e)
        : base(message, e)
    {
    }
}