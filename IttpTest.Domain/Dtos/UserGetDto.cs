namespace IttpTest.Web.Dtos;

public class UserGetDto
{
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }
}