using PMS.Application.DTOs.Auth;
using PMS.Domain.Entities;
using System.Security.Claims;

namespace PMS.Application.Interfaces.JwtService;

public interface IJwtService
{
    TokensDTO GenerateTokens(User user);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
