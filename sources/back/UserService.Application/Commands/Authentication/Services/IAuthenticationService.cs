using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;

namespace UserService.Application.Commands.Authentication.Services;

public interface IAuthenticationService
{
  Task<LoginResult> LoginAsync(LoginUserCommand command, CancellationToken ct);

  Task<LoginResult> RefreshAsync(string refreshToken, CancellationToken ct);

  Task LogoutAsync(string refreshToken, CancellationToken ct);
}