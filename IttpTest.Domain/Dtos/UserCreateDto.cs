namespace IttpTest.Domain.Dtos;

public class UserCreateDto
{
    public UserCreateDto(string login, string password, string name, int gender, DateTime? birthDate, bool admin)
    {
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        BirthDate = birthDate;
        Admin = admin;
    }

    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; } 
    public bool Admin { get; set; }
}