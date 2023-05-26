using IttpTest.Domain.Dtos;
using IttpTest.Domain.Models;

namespace IttpTest.Core;

public interface IUserService
{
    CookieDto SignIn(string login, string password);
    Task Create(UserCreateDto userCreateDto, Guid creatorId);
    Task Update(UserUpdateDto userUpdateDto, Guid modifierId);
    Task ChangeLogin(ChangeLoginDto changeLoginDto, Guid modifierId);
    Task<List<UserGetFullDto>> GetNotRevoked();
    UserGetDto GetByLogin(string login);
    UserGetFullDto GetByLoginAndPassword(string login, string password);
    Task<List<UserGetFullDto>> GetOlderThen(DateTime age);
    Task Revoke(string login, Guid revokerId);
    Task Delete(string login);
    Task Restore(string login);
}