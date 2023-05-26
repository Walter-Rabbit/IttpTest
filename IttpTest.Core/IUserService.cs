using IttpTest.Domain.Models;
using IttpTest.Web.Dtos;

namespace IttpTest.Core;

public interface IUserService
{
    CookieDto SignIn(string login, string password);
    Task Create(UserCreateDto userCreateDto, string creatorLogin);
    Task Create(UserCreateByAdminDto userCreateByAdminDto, string creatorLogin);
    Task Update(UserUpdateDto userUpdateDto, string modifierLogin);
    Task<List<UserGetDto>> GetNotRevoked();
    Task<UserGetDto> GetByLogin(string login);
    UserGetDto GetByLoginAndPassword(string login, string password);
    Task<List<UserGetDto>> GetOlderThen(DateTime age);
    Task Revoke(string login, string revokerLogin);
    Task Delete(string login);
    Task Restore(string login);
}