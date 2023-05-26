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
        var user = _userService.GetByLoginAndPassword(signInDto.Login, signInDto.Password);
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

        await _userService.Create(user);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Policy = "Admin")]
    [HttpPost("create-by-admin")]
    public async Task Create(UserCreateByAdminDto userCreateByAdminDto)
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
            HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Login")?.Value ??
            throw new InternalException("There is no login claim in cookie."));

        await _userService.Create(user);
    }
}