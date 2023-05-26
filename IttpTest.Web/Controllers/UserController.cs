using System.Security.Claims;
using IttpTest.Core;
using IttpTest.Domain.Exceptions;
using IttpTest.Domain.Models;
using IttpTest.Web.Dtos;
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

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("sign-in")]
    public async Task Login([FromBody] SignInDto signInDto)
    {
        var user = _userService.SignIn(signInDto.Login, signInDto.Password);
        var claims = new List<Claim>
        {
            new Claim("Name", user.Name),
            new Claim("Login", user.Login),
            new Claim("Id", user.Id.ToString()),
            new Claim("Role", user.Admin.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpPost("create")]
    public async Task Create(UserCreateDto userCreateDto)
    {
        await _userService.Create(userCreateDto, userCreateDto.Login);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "Admin")]
    [HttpPost("create-by-admin")]
    public async Task Create(UserCreateByAdminDto userCreateByAdminDto)
    {
        await _userService.Create(
            userCreateByAdminDto,
            HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
            throw new InternalException("There is no login claim in cookie."));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpPatch("update")]
    public async Task Update(UserUpdateDto userUpdateDto)
    {
        await _userService.Update(userUpdateDto,
            HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
            throw new InternalException("There is no login claim in cookie."));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpGet("get-not-revoked")]
    public async Task<List<UserGetDto>> GetNotRevoked()
    {
        return await _userService.GetNotRevoked();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpGet("get/{login}")]
    public async Task<UserGetDto> Get(string login)
    {
        return await _userService.GetByLogin(login);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("get")]
    public UserGetDto Get(SignInDto signInDto)
    {
        return _userService.GetByLoginAndPassword(signInDto.Login, signInDto.Password);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpGet("get-older-then")]
    public Task<List<UserGetDto>> GetOlderThen(DateTime age)
    {
        return _userService.GetOlderThen(age);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpDelete("revoke")]
    public async Task Revoke(string login)
    {
        await _userService.Revoke(login,
            HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
            throw new InternalException("There is no login claim in cookie."));
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpDelete("delete")]
    public async Task Delete(string login)
    {
        await _userService.Delete(login);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "Admin")]
    [HttpPatch("restore")]
    public async Task Restore(string login)
    {
        await _userService.Restore(login);
    }
}