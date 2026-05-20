using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PMS.Application.DTOs.Auth;
using PMS.Application.DTOs.Responses;
using PMS.Application.Exceptions;
using PMS.Application.Interfaces.AuthService;
using PMS.Application.Interfaces.EmailService;
using PMS.Application.Interfaces.JwtService;
using PMS.Domain.Entities;
using PMS.Infrastructure.Settings;
using System.Security.Claims;
using System.Text;

namespace PMS.Infrastructure.Services.AuthService;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtGenerator;
    private readonly IEmailService _emailService;
    private readonly URLs _urlsConfiguration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        IJwtService jwtGenerator,
        IEmailService emailService,
        IOptions<URLs> urlsConfiguration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
        _emailService = emailService;
        _urlsConfiguration = urlsConfiguration.Value;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> RegisterAsync(RegisterDTO dto, CancellationToken cancellationToken = default)
    {
        var user = new User { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            throw new BadRequestException("Registration failed.", result.Errors.Select(e => e.Description));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmationLink = $"{_urlsConfiguration.Backend}/api/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";
        try
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"<a href='{confirmationLink}'>Click here to confirm your email</a>",
                cancellationToken);

            _logger.LogInformation("Successfully the email sent for user {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send confirmation email for user {Email}", user.Email);

            await _userManager.DeleteAsync(user);

            throw new ServiceUnavailableException("We experienced an issue sending your confirmation email. Your registration has been cancelled, please try again later.");
        }

        return ApiResponse<bool>.Success(true, "User registered successfully. Please check your email.");
    }

    public async Task<ApiResponse<TokensDTO>> LoginAsync(LoginDTO dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            throw new UnauthorizedException("Invalid credentials.");

        if (await _userManager.IsLockedOutAsync(user))
        {
            var lockoutEnd = user.LockoutEnd?.UtcDateTime ?? DateTime.UtcNow;
            var secondsLeft = (int)Math.Ceiling((lockoutEnd - DateTime.UtcNow).TotalSeconds);

            throw new UnauthorizedException($"Account is locked. Try again in {secondsLeft} second(s).");
        }

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            await _userManager.AccessFailedAsync(user);
            throw new UnauthorizedException("Invalid credentials.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        if (!await _userManager.IsEmailConfirmedAsync(user))
            throw new UnauthorizedException("Please confirm your email before logging in.");

        var tokens = _jwtGenerator.GenerateTokens(user);

        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", tokens.RefreshToken);

        _logger.LogInformation("{Email} Logined Successfully", user.Email);

        return ApiResponse<TokensDTO>.Success(tokens, "Login Successfully. Welcome back!");
    }

    public async Task<ApiResponse<TokensDTO>> RefreshTokenAsync(string expiredToken, string refreshToken, CancellationToken cancellationToken = default)
    {
        var principal = _jwtGenerator.GetPrincipalFromExpiredToken(expiredToken);
        if (principal == null)
            throw new UnauthorizedException("Invalid access token.");

        var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString))
            throw new UnauthorizedException("Invalid access token.");

        var user = await _userManager.FindByIdAsync(userIdString);
        if (user == null)
            throw new UnauthorizedException("Invalid access token.");

        var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
        if (storedRefreshToken != refreshToken)
            throw new UnauthorizedException("Invalid refresh token.");

        var newTokens = _jwtGenerator.GenerateTokens(user);

        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newTokens.RefreshToken);

        return ApiResponse<TokensDTO>.Success(newTokens, "Tokens refreshed successfully.");
    }

    public async Task<ApiResponse<bool>> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found.");

        try
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                throw new BadRequestException("Email confirmation failed.", result.Errors.Select(x => x.Description));

            _logger.LogInformation("Successfully confirmed email for user {Email}", user.Email);

            return ApiResponse<bool>.Success(true, "Email confirmed successfully.");
        }
        catch (BadRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm email for user {Email}", user.Email);
            throw new BadRequestException("Email confirmation failed.");
        }
    }
}
