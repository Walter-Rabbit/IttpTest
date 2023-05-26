using System.Data.Entity;
using IttpTest.Data;
using IttpTest.Domain.Exceptions;
using IttpTest.Domain.Models;
using IttpTest.Web.Dtos;
using Microsoft.Extensions.Configuration;

namespace IttpTest.Core;

public class UserService : IUserService
{
    private readonly IttpContext _ittpContext;

    public UserService(IttpContext ittpContext, IConfiguration configuration)
    {
        _ittpContext = ittpContext;
        _ittpContext.Users.Add(new User(
            Guid.Empty,
            configuration["RootUser:Login"] ??
            throw new InternalException("Missing parameter in configuration: RootUser:Login"),
            configuration["RootUser:Password"] ??
            throw new InternalException("Missing parameter in configuration: RootUser:Password"),
            configuration["RootUser:Name"] ??
            throw new InternalException("Missing parameter in configuration: RootUser:Name"),
            Convert.ToInt32(configuration["RootUser:Gender"] ??
                            throw new InternalException("Missing parameter in configuration: RootUser:Gender")),
            null,
            true,
            DateTime.Now,
            configuration["RootUser:Login"] ??
            throw new InternalException("Missing parameter in configuration: RootUser:Login")));

        _ittpContext.SaveChanges();
    }

    public UserService(IttpContext ittpContext)
    {
        _ittpContext = ittpContext;
    }

    public CookieDto SignIn(string login, string password)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        if (user.RevokedOn is not null)
        {
            throw new RevokedException($"This user was revoked on {user.RevokedOn}");
        }
        
        if (user.Password != password)
        {
            throw new IncorrectPasswordException("Incorrect password.");
        }

        return new CookieDto
        {
            Name = user.Name,
            Login = user.Login,
            Admin = user.Admin,
            Id = user.Id,
        };
    }

    public Task Create(UserCreateDto userCreateDto, string creatorLogin)
    {
        var user = new User(
            Guid.NewGuid(),
            userCreateDto.Login,
            userCreateDto.Password,
            userCreateDto.Name,
            userCreateDto.Gender,
            userCreateDto.BirthDate,
            false,
            DateTime.Now,
            userCreateDto.Login);

        throw new NotImplementedException();
    }

    public Task Create(UserCreateByAdminDto userCreateByAdminDto, string creatorLogin)
    {
        var user = new User(
            Guid.NewGuid(),
            userCreateByAdminDto.Login,
            userCreateByAdminDto.Password,
            userCreateByAdminDto.Name,
            userCreateByAdminDto.Gender,
            userCreateByAdminDto.BirthDate,
            userCreateByAdminDto.Admin,
            DateTime.Now,
            creatorLogin);

        throw new NotImplementedException();
    }

    public Task Update(UserUpdateDto userUpdateDto, string modifierLogin)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserGetDto>> GetNotRevoked()
    {
        throw new NotImplementedException();
    }

    public Task<UserGetDto> GetByLogin(string login)
    {
        throw new NotImplementedException();
    }

    public UserGetDto GetByLoginAndPassword(string login, string password)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }
        
        if (user.RevokedOn is not null)
        {
            throw new RevokedException($"This user was revoked on {user.RevokedOn}");
        }

        if (user.Password != password)
        {
            throw new IncorrectPasswordException("Incorrect password.");
        }

        return new UserGetDto
        {
            Name = user.Name,
            BirthDate = user.BirthDate,
            Gender = user.Gender,
            IsActive = user.ModifiedOn is null
        };
    }

    public Task<List<UserGetDto>> GetOlderThen(DateTime age)
    {
        throw new NotImplementedException();
    }

    public Task Revoke(string login, string revokerLogin)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string login)
    {
        throw new NotImplementedException();
    }

    public Task Restore(string login)
    {
        throw new NotImplementedException();
    }
}