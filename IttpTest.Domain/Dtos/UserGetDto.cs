namespace IttpTest.Domain.Dtos;

public class UserGetDto
{
    public UserGetDto(string name, int gender, DateTime? birthDate, bool isActive)
    {
        Name = name;
        Gender = gender;
        BirthDate = birthDate;
        IsActive = isActive;
    }

    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }
}