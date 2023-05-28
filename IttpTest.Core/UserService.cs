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

        var rootId = Guid.NewGuid();
        if (_ittpContext.Users.FirstOrDefault(u => u.Login == configuration["RootUser:Login"]) is not null) return;
        _ittpContext.Users.Add(new User(
            rootId,
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
            rootId));

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

        return new CookieDto(user.Id, user.Login, user.Admin);
    }

    public async Task Create(UserCreateDto userCreateDto, Guid creatorId)
    {
        var user = new User(
            Guid.NewGuid(),
            userCreateDto.Login,
            userCreateDto.Password,
            userCreateDto.Name,
            userCreateDto.Gender,
            userCreateDto.BirthDate,
            userCreateDto.Admin,
            DateTime.Now,
            creatorId);

        if (_ittpContext.Users.FirstOrDefault(u => u.Login == user.Login) is not null)
        {
            throw new LoginAlreadyExistsException($"Login {user.Login} already occupied.");
        }

        await _ittpContext.Users.AddAsync(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task Update(UserUpdateDto userUpdateDto, Guid modifierId)
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
        user.ModifierId = modifierId;

        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task ChangeLogin(ChangeLoginDto changeLoginDto, Guid modifierId)
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
        user.ModifierId = modifierId;
        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    public async Task<List<UserGetFullDto>> GetNotRevoked()
    {
        return await _ittpContext.Users
            .Where(u => u.RevokedOn == null)
            .Select(u => new UserGetFullDto(
                u.Id,
                u.Login,
                u.Password,
                u.Name,
                u.Gender,
                u.BirthDate,
                u.Admin,
                u.CreatedOn,
                _ittpContext.Users.First(creator => creator.Id == u.CreatorId).Login,
                u.ModifiedOn,
                GetLogin(u.ModifierId),
                u.RevokedOn,
                GetLogin(u.RevokerId)
            ))
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

    public UserGetFullDto GetByLoginAndPassword(string login, string password)
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

        return new UserGetFullDto(
            user.Id,
            user.Login,
            user.Password,
            user.Name,
            user.Gender,
            user.BirthDate,
            user.Admin,
            user.CreatedOn,
            _ittpContext.Users.First(creator => creator.Id == user.CreatorId).Login,
            user.ModifiedOn,
            GetLogin(user.ModifierId),
            user.RevokedOn,
            GetLogin(user.RevokerId));
    }

    public async Task<List<UserGetFullDto>> GetOlderThen(DateTime birthDate)
    {
        return await _ittpContext.Users
            .Where(u => u.BirthDate != null && u.BirthDate > birthDate)
            .Select(u => new UserGetFullDto(
                u.Id,
                u.Login,
                u.Password,
                u.Name,
                u.Gender,
                u.BirthDate,
                u.Admin,
                u.CreatedOn,
                _ittpContext.Users.First(creator => creator.Id == u.CreatorId).Login,
                u.ModifiedOn,
                GetLogin(u.ModifierId),
                u.RevokedOn,
                GetLogin(u.RevokerId)
            ))
            .ToListAsync();
    }

    public async Task Revoke(string login, Guid revokerId)
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
        user.RevokerId = revokerId;

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
        user.RevokerId = null;

        _ittpContext.Users.Update(user);
        await _ittpContext.SaveChangesAsync();
    }

    private string? GetLogin(Guid? id)
    {
        return _ittpContext.Users.FirstOrDefault(modifier => modifier.Id == id)?.Login;
    }
}