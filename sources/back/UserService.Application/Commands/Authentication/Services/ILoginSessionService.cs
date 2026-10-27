using UserService.Domain.Authentication;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication.Services;

public interface ILoginSessionService
{
  UserSession CreateSession(User user, string? deviceId);

  Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken ct);

  Task PersistAsync(UserSession session, CancellationToken ct);

  Task UpdateAsync(UserSession session, CancellationToken ct);
}