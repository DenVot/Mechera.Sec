using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mechera.Sec.Authorization.Controllers;

[ApiController]
//[Authorize(Roles = "Root")]
[Route("/api/users/")]
public class UserManagingController : ControllerBase
{
    private readonly IUserManager _userManager;

    public UserManagingController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetAllUsers() => 
        Ok(_userManager.GetUsers().Select(UserInfoEntity.Create));

    [HttpPost("create")]
    public async Task<IActionResult> CreateNewUser([FromBody] AuthEntity authData)
    {
        var user = await _userManager.CreateUserAsync(authData.Username, authData.Password);

        if (user == null) return BadRequest("Invalid auth data");

        return Ok();
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteUser([FromQuery] string username)
    {
        if (username == "Root") return BadRequest("Can't remove the root user");

        try
        {
            await _userManager.RemoveUserAsync(username);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Invalid data");
        }
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] AuthEntity authData)
    {
        try
        {
            await _userManager.UpdatePasswordAsync(authData.Username, authData.Password);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Invalid data");
        }
    }
}
