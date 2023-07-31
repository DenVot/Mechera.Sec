using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data.Repositories;
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
    private readonly IUsersRepository _usersRepository;

    public UsersController(IJwtGenerator jwtGenerator, 
        IUserAuthenticator userAuthenticator,
        IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
        _jwtGenerator = jwtGenerator;
        _userAuthenticator = userAuthenticator;
        _usersRepository = usersRepository;
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
            roleClaim == null)
        {
            return Unauthorized();
        }

        var targetUser = await _usersRepository.GetAsync(long.Parse(nameClaim.Value));

        if (targetUser == null) return Unauthorized();

        return Ok(new UserInfoEntity(targetUser.Id, targetUser.Username, roleClaim.Value));
    }
}
