using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
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
        [FromBody] AuthEntity auth)
    { 
        throw new NotImplementedException();
    }

    /// <summary>
    /// Производит операцию верифакации токена
    /// </summary>
    /// <param name="jwt">Токен</param>    
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyToken(
        [FromQuery] string jwt)
    {
        throw new NotImplementedException();
    }
}
