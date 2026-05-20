using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PMS.Application.DTOs.Auth;
using PMS.Application.DTOs.Responses;
using PMS.Application.Exceptions;
using PMS.Application.Interfaces.AuthService;
using PMS.Infrastructure.Settings;

namespace PMS.API.Controllers;

public class AccountController : AppBaseController
{
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AccountController(IAuthService authService, IOptions<JwtSettings> jwtSettings)
    {
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ApiResponse<bool>>> Register([FromBody] RegisterDTO request, CancellationToken cancellationToken = default)
    {
        return StatusCode(StatusCodes.Status201Created, await _authService.RegisterAsync(request, cancellationToken));
    }

    [HttpGet("ConfirmEmail")]
    public async Task<ActionResult<ApiResponse<bool>>> ConfirmEmail([FromQuery] string userId, [FromQuery] string token, CancellationToken cancellationToken = default)
    {
        return Ok(await _authService.ConfirmEmailAsync(userId, token, cancellationToken));
    }

    [HttpPost("Login"), AllowAnonymous]
    public async Task<ActionResult<ApiResponse<bool>>> Login([FromBody] LoginDTO request, CancellationToken cancellationToken = default)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (result.Data is null)
        {
            throw new InvalidOperationException("Result data is null.");
        }

        SetTokenCookies(result.Data.AccessToken, result.Data.RefreshToken);

        return Ok(ApiResponse<bool>.Success(true, result.Message));
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<ApiResponse<string>>> RefreshToken(CancellationToken cancellationToken = default)
    {
        var expiredAccessToken = Request.Cookies[Constants.AccessTokenName];
        var refreshToken = Request.Cookies[Constants.RrefreshTokenName];

        if (string.IsNullOrEmpty(expiredAccessToken) || string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("Tokens missing from cookies.");

        var result = await _authService.RefreshTokenAsync(expiredAccessToken, refreshToken, cancellationToken);

        if (result.Data is null)
        {
            throw new InvalidOperationException("Result data is null.");
        }

        SetTokenCookies(result.Data.AccessToken, result.Data.RefreshToken);

        return Ok(ApiResponse<string>.Success("Tokens refreshed successfully."));
    }

    [HttpPost("logout")]
    public ActionResult<ApiResponse<bool>> Logout()
    {
        Response.Cookies.Delete(Constants.AccessTokenName);
        Response.Cookies.Delete(Constants.RrefreshTokenName);

        return Ok(ApiResponse<bool>.Success(true));
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        var refreshCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
        };

        Response.Cookies.Append(Constants.AccessTokenName, accessToken, cookieOptions);
        Response.Cookies.Append(Constants.RrefreshTokenName, refreshToken, refreshCookieOptions);
    }
}
