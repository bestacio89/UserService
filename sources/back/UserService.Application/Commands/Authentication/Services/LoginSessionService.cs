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
    var session = _sessionFactory.Create();

    session.Define(user.Id, deviceId);

    return session;
  }

  public async Task PersistAsync(UserSession session, CancellationToken ct)
  {
    await _sessions.AddAsync(session, ct);
  }
}