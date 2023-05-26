namespace IttpTest.Domain.Dtos;

public class SignInDto
{
    public SignInDto(string login, string password)
    {
        Login = login;
        Password = password;
    }

    public string Login { get; set; }
    public string Password { get; set; }
}