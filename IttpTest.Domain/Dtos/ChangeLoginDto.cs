namespace IttpTest.Domain.Dtos;

public class ChangeLoginDto
{
    public ChangeLoginDto(string oldLogin, string newLogin)
    {
        OldLogin = oldLogin;
        NewLogin = newLogin;
    }

    public string OldLogin { get; set; }
    public string NewLogin { get; set; }
}