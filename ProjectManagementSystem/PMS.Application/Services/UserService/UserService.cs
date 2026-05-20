using Microsoft.AspNetCore.Identity;
using PMS.Application.DTOs.Responses;
using PMS.Application.DTOs.User;
using PMS.Application.Exceptions;
using PMS.Domain.Entities;

namespace PMS.Application.Services.UserService;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApiResponse<UserDTO>> GetInfo(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new NotFoundException("User not found.");

        var result = new UserDTO
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!
        };

        return ApiResponse<UserDTO>.Success(result);
    }
}
