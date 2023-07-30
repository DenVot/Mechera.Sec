﻿using Mechera.Sec.Authorization.Entities;
using Mechera.Sec.Authorization.Tools;
using Mechera.Sec.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mechera.Sec.Authorization.Controllers;

[ApiController]
[Authorize(Roles = "Root")]
[Route("/api/users/")]
public class UserManagingController : ControllerBase
{
    private readonly IUserManager _userManager;
    private readonly IUsersRepository _usersRepository;

    public UserManagingController(IUserManager userManager, IUsersRepository usersRepository)
    {
        _userManager = userManager;
        _usersRepository = usersRepository;
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
    public async Task<IActionResult> DeleteUser([FromQuery] long id)
    {
        var targetUser = await _usersRepository.GetAsync(id);

        if (targetUser == null) return BadRequest();
        if (targetUser.Username == "Root") return BadRequest();

        try
        {
            await _userManager.RemoveUserAsync(id);

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
