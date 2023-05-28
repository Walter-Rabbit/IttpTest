namespace IttpTest.Domain.Dtos;

public class UserGetFullDto
{
    public UserGetFullDto(
        Guid id,
        string login,
        string password,
        string name,
        int gender,
        DateTime? birthDate,
        bool admin,
        DateTime createdOn,
        string createdBy,
        DateTime? modifiedOn,
        string? modifiedBy,
        DateTime? revokedOn,
        string? revokedBy)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        BirthDate = birthDate;
        Admin = admin;
        CreatedOn = createdOn;
        CreatedBy = createdBy;
        ModifiedOn = modifiedOn;
        ModifiedBy = modifiedBy;
        RevokedOn = revokedOn;
        RevokedBy = revokedBy;
    }

    public Guid Id { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public string Name { get; set; }

    public int Gender { get; set; }

    public DateTime? BirthDate { get; set; }
    public bool Admin { get; set; }
    public DateTime CreatedOn { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime? RevokedOn { get; set; }
    public string? RevokedBy { get; set; }
}