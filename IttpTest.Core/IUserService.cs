using IttpTest.Domain.Models;

namespace IttpTest.Core;

public interface IUserService
{
    Task Create(User user);
    Task Update(User user);
    Task<List<User>> GetNotRevoked();
    Task<User> GetByLogin(string login);
    User GetByLoginAndPassword(string login, string password);
    Task<List<User>> GetOlderThen(DateOnly age);
    Task Revoke(string login);
    Task Delete(string login);
    Task Restore(string login);
}