using PMS.Application.DTOs.Auth;
using PMS.Application.DTOs.Responses;

namespace PMS.Application.Interfaces.AuthService;

public interface IAuthService
{
    Task<ApiResponse<bool>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<TokensDTO>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default);
    Task<ApiResponse<TokensDTO>> RefreshTokenAsync(string expiredToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
}
