using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMS.Application.DTOs.Responses;
using PMS.Application.DTOs.User;
using PMS.Application.Services.UserService;

namespace PMS.API.Controllers;

[Authorize]
public class UserController : AppBaseController
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("GetInfo")]
    public async Task<ActionResult<ApiResponse<UserDTO>>> GetInfo(CancellationToken cancellationToken = default)
    {
        return Ok(await _userService.GetInfo(UserId, cancellationToken));
    }
}
