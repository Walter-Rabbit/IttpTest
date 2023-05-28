namespace IttpTest.Domain.Dtos;

public class CookieDto
{
    public CookieDto(Guid id, string login, bool admin)
    {
        Id = id;
        Login = login;
        Admin = admin;
    }

    public Guid Id { get; set; }
    public string Login { get; set; }
    public bool Admin { get; set; }
}