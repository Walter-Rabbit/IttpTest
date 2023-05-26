namespace IttpTest.Web.Dtos;

public class UserCreateByAdminDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; } 
    public bool Admin { get; set; }
}