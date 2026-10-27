using System.Security.Claims;
using UserService.Domain.Authentication;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication.Services;

public interface ITokenService
{
  string CreateAccessToken(User user, UserSession session);

  string CreateRefreshToken(User user, UserSession session);

  ClaimsPrincipal ValidateRefreshToken(string token, bool validateLifetime = true);

  string HashRefreshToken(string token);

  DateTime GetAccessTokenExpiry();

  DateTime GetRefreshTokenExpiry();
}