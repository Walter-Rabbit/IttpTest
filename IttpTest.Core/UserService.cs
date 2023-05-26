using System.Data.Entity;
using IttpTest.Data;
using IttpTest.Domain.Exceptions;
using IttpTest.Domain.Models;
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

    public Task Create(User user)
    {
        throw new NotImplementedException();
    }

    public Task Update(User user)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetNotRevoked()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetByLogin(string login)
    {
        throw new NotImplementedException();
    }

    public User GetByLoginAndPassword(string login, string password)
    {
        var user = _ittpContext.Users.FirstOrDefault(u => u.Login == login);

        if (user is null)
        {
            throw new NotFoundException("There is no user with such login.");
        }

        if (user.Password != password)
        {
            throw new IncorrectPasswordException("Incorrect password.");
        }

        return user;
    }

    public Task<List<User>> GetOlderThen(DateOnly age)
    {
        throw new NotImplementedException();
    }

    public Task Revoke(string login)
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