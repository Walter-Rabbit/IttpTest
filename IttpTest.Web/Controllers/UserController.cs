using System.Security.Claims;
using IttpTest.Core;
using IttpTest.Domain.Dtos;
using IttpTest.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IttpTest.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("sign-in")]
    public async Task Login([FromBody] SignInDto signInDto)
    {
        var user = _userService.SignIn(signInDto.Login, signInDto.Password);
        var claims = new List<Claim>
        {
            new Claim("Login", user.Login),
            new Claim("Id", user.Id.ToString()),
            new Claim("IsAdmin", user.Admin.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("create")]
    public async Task Create(UserCreateDto userCreateDto)
    {
        await _userService.Create(
            userCreateDto,
            Guid.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ??
                       throw new InternalException("There is no Id claim in cookie.")));
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task Update(UserUpdateDto userUpdateDto)
    {
        var modifierLogin = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
                            throw new InternalException("There is no Login claim in cookie.");

        var modifierId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ??
                                    throw new InternalException("There is no Id claim in cookie."));

        var modifierIsAdmin = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "IsAdmin")?.Value ??
                              throw new InternalException("There is no IsAdmin claim in cookie.");

        if (modifierLogin != userUpdateDto.Login && modifierIsAdmin != "True")
        {
            throw new ForbiddenException("You can change only yours profile information.");
        }

        if (modifierLogin != userUpdateDto.Login && 
            modifierLogin != _configuration["RootUser:Login"] && 
            _userService.IsAdmin(userUpdateDto.Login))
        {
            throw new ForbiddenException("You can't change profile information of other admin.");
        }

        if (userUpdateDto.Login == _configuration["RootUser:Login"])
        {
            throw new ForbiddenException("You can't change root information.");
        }

        await _userService.Update(userUpdateDto, modifierId);
    }

    [Authorize]
    [HttpPatch("update/login")]
    public async Task ChangeLogin(ChangeLoginDto changeLoginDto)
    {
        var modifierLogin = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
                            throw new InternalException("There is no Login claim in cookie.");

        var modifierId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ??
                                    throw new InternalException("There is no Id claim in cookie."));

        var modifierIsAdmin = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "IsAdmin")?.Value ??
                              throw new InternalException("There is no IsAdmin claim in cookie.");

        if (modifierLogin != changeLoginDto.OldLogin && modifierIsAdmin != "True")
        {
            throw new ForbiddenException("You can change only yours profile information.");
        }

        if (modifierLogin != changeLoginDto.OldLogin && 
            modifierLogin != _configuration["RootUser:Login"] && 
            _userService.IsAdmin(changeLoginDto.OldLogin))
        {
            throw new ForbiddenException("You can't change profile information of other admin.");
        }

        if (changeLoginDto.OldLogin == _configuration["RootUser:Login"])
        {
            throw new ForbiddenException("You can't change root login.");
        }

        await _userService.ChangeLogin(changeLoginDto, modifierId);

        if (changeLoginDto.OldLogin == modifierLogin)
        {
            var claims = new List<Claim>
            {
                new Claim("Login", changeLoginDto.NewLogin),
                new Claim("Id", modifierId.ToString()),
                new Claim("IsAdmin", modifierIsAdmin),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
        }
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("get-not-revoked")]
    public async Task<List<UserGetFullDto>> GetNotRevoked()
    {
        return await _userService.GetNotRevoked();
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("get/{login}")]
    public UserGetDto Get(string login)
    {
        return _userService.GetByLogin(login);
    }

    [HttpGet("get")]
    public UserGetFullDto Get(string login, string password)
    {
        return _userService.GetByLoginAndPassword(login, password);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("get-older-then")]
    public Task<List<UserGetFullDto>> GetOlderThen(DateTime birthDate)
    {
        return _userService.GetOlderThen(birthDate);
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("revoke")]
    public async Task Revoke(string login)
    {
        await _userService.Revoke(
            login,
            Guid.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ??
                       throw new InternalException("There is no Id claim in cookie.")));
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("delete")]
    public async Task Delete(string login)
    {
        await _userService.Delete(login);
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("restore")]
    public async Task Restore(string login)
    {
        await _userService.Restore(login);
    }
}