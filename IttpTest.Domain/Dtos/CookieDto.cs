namespace IttpTest.Domain.Dtos;

public class CookieDto
{
    public CookieDto(Guid id, string login, string name, bool admin)
    {
        Id = id;
        Login = login;
        Name = name;
        Admin = admin;
    }

    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Name { get; set; }
    public bool Admin { get; set; }
}