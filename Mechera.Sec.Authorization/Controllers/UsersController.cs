using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mechera.Sec.Authorization.Controllers;

[Route("api/auth")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IUserAuthenticator _userAuthenticator;

    public UsersController(IJwtGenerator jwtGenerator, IUserAuthenticator userAuthenticator)
    {
        _jwtGenerator = jwtGenerator;
        _userAuthenticator = userAuthenticator;
    }

    /// <summary>
    /// Производит операцию входа
    /// </summary>
    /// <param name="auth">Аунтефикационные параметры</param>    
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] AuthEntity auth, 
        [FromQuery] string redirectOnSuccess)
    { 
        var targetUser = await _userAuthenticator.AuthenticateAsync(auth.Username, auth.Password);

        if (targetUser == null) return Unauthorized();        

        var jwt = _jwtGenerator.GenerateToken(targetUser);

        return Redirect(redirectOnSuccess + $"jwt={jwt}");
    }

    /// <summary>
    /// Производит операцию верифакации токена
    /// </summary>    
    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyToken()
    {
        var usernameClaim = User.FindFirst("username");
        var roleClaim = User.FindFirst("role");

        if (usernameClaim == null || 
            roleClaim == null ||
            !await _userAuthenticator.CheckIfUserExistsAsync(usernameClaim.Value))
        {
            return Unauthorized();
        }

        return Ok(new UserInfoEntity(usernameClaim.Value, roleClaim.Value));
    }
}
