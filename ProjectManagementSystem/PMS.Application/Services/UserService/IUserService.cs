using PMS.Application.DTOs.Responses;
using PMS.Application.DTOs.User;

namespace PMS.Application.Services.UserService;

public interface IUserService
{
    Task<ApiResponse<UserDTO>> GetInfo(Guid userId, CancellationToken cancellationToken = default);
}
