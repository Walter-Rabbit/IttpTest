using IttpTest.Data;
using IttpTest.Domain.Dtos;
using IttpTest.Domain.Exceptions;
using IttpTest.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IttpTest.Core;

public class UserService : IUserService
{
    private readonly IttpContext _ittpContext;

    public UserService(IttpContext ittpContext, IConfiguration configuration)
    {
        _ittpContext = ittpContext;

        if (_ittpContext.Users.FirstOrDefault(u => u.Login == configuration["RootUser:Login"]) is not null) return;
        _ittpContext.Users.Add(new User(
            Guid.NewGuid(),
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

        return new CookieDto(user.Id, user.Login, user.Name, user.Admin);
    }

    public async Task Create(UserCreateDto userCreateDto, string creatorLogin)
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

        if (_ittpContext.Users.FirstOrDefault(u => u.Login == user.Login) is not null)
        {
            throw new LoginAlreadyExistsException($"Login {user.Login} already occupied.");
        }

        await _ittpContext.Users.AddAsync(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task Create(UserCreateByAdminDto userCreateByAdminDto, string creatorLogin)
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

        if (_ittpContext.Users.FirstOrDefault(u => u.Login == user.Login) is not null)
        {
            throw new LoginAlreadyExistsException($"Login {user.Login} already occupied.");
        }

        await _ittpContext.Users.AddAsync(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task Update(UserUpdateDto userUpdateDto, string modifierLogin)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == userUpdateDto.Login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        if (user.RevokedOn is not null)
        {
            throw new RevokedException($"This user was revoked on {user.RevokedOn}");
        }

        user.Name = userUpdateDto.Name;
        user.Password = userUpdateDto.Password;
        user.BirthDate = userUpdateDto.BirthDate;
        user.Gender = userUpdateDto.Gender;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = modifierLogin;

        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task ChangeLogin(ChangeLoginDto changeLoginDto, string modifierLogin)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == changeLoginDto.OldLogin);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        if (user.RevokedOn is not null)
        {
            throw new RevokedException($"This user was revoked on {user.RevokedOn}");
        }
        
        if (_ittpContext.Users.FirstOrDefault(u => u.Login == changeLoginDto.NewLogin) is not null)
        {
            throw new LoginAlreadyExistsException($"Login {changeLoginDto.NewLogin} already occupied.");
        }

        user.Login = changeLoginDto.NewLogin;
        user.ModifiedOn = DateTime.Now;
        user.ModifiedBy = modifierLogin;
        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task<List<User>> GetNotRevoked()
    {
        return await _ittpContext.Users
            .Where(u => u.RevokedOn == null)
            .ToListAsync();
    }

    public UserGetDto GetByLogin(string login)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        return new UserGetDto(user.Name, user.Gender, user.BirthDate, user.RevokedOn is null);
    }

    public User GetByLoginAndPassword(string login, string password)
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

        return user;
    }

    public async Task<List<User>> GetOlderThen(DateTime age)
    {
        // TODO: Заменить age на дату рождения
        return await _ittpContext.Users
            .Where(u => u.BirthDate != null && (DateTime.Now - u.BirthDate).Value.Ticks > age.Ticks)
            .ToListAsync();
    }

    public async Task Revoke(string login, string revokerLogin)
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

        if (user.Admin)
        {
            throw new ForbiddenException("Only users can be revoked.");
        }

        user.RevokedOn = DateTime.Now;
        user.RevokedBy = revokerLogin;

        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task Delete(string login)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }
        
        if (user.Admin)
        {
            throw new ForbiddenException("Only users can be deleted.");
        }


        _ittpContext.Users.Remove(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task Restore(string login)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        user.RevokedOn = null;
        user.RevokedBy = null;

        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }
}