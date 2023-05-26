namespace IttpTest.Domain.Dtos;

public class UserUpdateDto
{
    public UserUpdateDto(string login, string password, string name, int gender, DateTime? birthDate)
    {
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        BirthDate = birthDate;
    }

    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}