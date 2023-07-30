using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [FromBody] AuthEntity auth)
    { 
        var targetUser = await _userAuthenticator.AuthenticateAsync(auth.Username, auth.Password);

        if (targetUser == null) return Unauthorized();        

        return Ok(_jwtGenerator.GenerateToken(targetUser));       
    }

    /// <summary>
    /// Производит операцию верифакации токена
    /// </summary>    
    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyToken()
    {
        var nameClaim = User.FindFirst(ClaimTypes.Name);
        var roleClaim = User.FindFirst(ClaimTypes.Role);

        if (nameClaim == null || 
            roleClaim == null ||
            !await _userAuthenticator.CheckIfUserExistsAsync(long.Parse(nameClaim.Value)))
        {
            return Unauthorized();
        }

        return Ok(new UserInfoEntity(nameClaim.Value, roleClaim.Value));
    }
}
