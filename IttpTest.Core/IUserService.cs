using IttpTest.Domain.Dtos;
using IttpTest.Domain.Models;

namespace IttpTest.Core;

public interface IUserService
{
    CookieDto SignIn(string login, string password);
    Task Create(UserCreateDto userCreateDto, string creatorLogin);
    Task Create(UserCreateByAdminDto userCreateByAdminDto, string creatorLogin);
    Task Update(UserUpdateDto userUpdateDto, string modifierLogin);
    Task ChangeLogin(ChangeLoginDto changeLoginDto, string modifierLogin);
    Task<List<User>> GetNotRevoked();
    UserGetDto GetByLogin(string login);
    User GetByLoginAndPassword(string login, string password);
    Task<List<User>> GetOlderThen(DateTime age);
    Task Revoke(string login, string revokerLogin);
    Task Delete(string login);
    Task Restore(string login);
}