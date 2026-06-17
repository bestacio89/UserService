using UserService.Domain.Users;
using UserService.Domain.Authentication;

namespace UserService.Application.Commands.Authentication.Services;

public interface ITokenService
{
  string CreateAccessToken(User user, UserSession session);

  string CreateRefreshToken(User user, UserSession session);

  DateTime GetAccessTokenExpiry();
}