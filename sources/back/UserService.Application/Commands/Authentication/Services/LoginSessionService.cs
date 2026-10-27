using Franz.Common.Business.Repositories;
using Franz.Common.Business.Domain.Factories;
using UserService.Domain.Authentication;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication.Services;

public sealed class LoginSessionService : ILoginSessionService
{
  private readonly IEntityFactory<Guid, UserSession> _sessionFactory;
  private readonly IEntityRepository<UserSession, Guid> _sessions;

  public LoginSessionService(
      IEntityFactory<Guid, UserSession> sessionFactory,
      IEntityRepository<UserSession, Guid> sessions)
  {
    _sessionFactory = sessionFactory;
    _sessions = sessions;
  }

  public UserSession CreateSession(User user, string? deviceId)
  {
    ArgumentNullException.ThrowIfNull(user);

    var session = _sessionFactory.Create();
    session.Define(user.Id, deviceId);
    return session;
  }

  public Task<UserSession?> GetByIdAsync(Guid sessionId, CancellationToken ct)
      => _sessions.GetByIdAsync(sessionId, ct);

  public Task PersistAsync(UserSession session, CancellationToken ct)
      => _sessions.AddAsync(session, ct);

  public Task UpdateAsync(UserSession session, CancellationToken ct)
      => _sessions.UpdateAsync(session, ct);
}