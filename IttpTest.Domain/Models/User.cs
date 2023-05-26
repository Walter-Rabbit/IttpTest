using System.Text.RegularExpressions;
using IttpTest.Domain.Exceptions;

namespace IttpTest.Domain.Models;

public class User
{
    private string _login;
    private string _password;
    private string _name;
    private int _gender;

    public User(
        Guid id, 
        string login, 
        string password, 
        string name,
        int gender, 
        DateTime? birthDate, 
        bool admin, 
        DateTime createdOn,
        Guid creatorId)
    {
        Id = id;
        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        BirthDate = birthDate;
        Admin = admin;
        CreatedOn = createdOn;
        CreatorId = creatorId;
    }

    public Guid Id { get; set; }

    public string Login
    {
        get => _login;
        set
        {
            if (!Regex.IsMatch(value, "^[a-zA-Z0-9]*$"))
            {
                throw new ValidationException("Login must contain only english letters and numbers.");
            }
            _login = value;
        }
    }

    public string Password
    {
        get => _password;
        set 
        {
            if (!Regex.IsMatch(value, "^[a-zA-Z0-9]*$"))
            {
                throw new ValidationException("Password must contain only english letters and numbers.");
            }
            _password = value;
        }
    }

    public string Name
    {
        get => _name;
        set
        {
            if (!Regex.IsMatch(value, "^[a-zA-Zа-яА-Я]*$"))
            {
                throw new ValidationException("Name must contain only english or cyrillic letters and numbers.");
            }
            _name = value;
        }
    }

    public int Gender
    {
        get => _gender;
        set
        {
            if (value is < 0 or > 2)
            {
                throw new ValidationException("Gender option is 0 for Female, 1 for Male and 2 for Undefined");
            }
            _gender = value;
        }
    }

    public DateTime? BirthDate { get; set; }
    public bool Admin { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? ModifierId { get; set; }
    public DateTime? RevokedOn { get; set; }
    public Guid? RevokerId { get; set; }
}