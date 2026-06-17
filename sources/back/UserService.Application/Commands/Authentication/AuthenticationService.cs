using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Commands.Authentication.Services;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Contracts.Persistence;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
  private readonly IAuthenticationProviderFactory _providerFactory;
  private readonly IUserIdentityLookupRepository _identityRepo;
  private readonly IEntityRepository<User, Guid> _userRepo;
  private readonly ILoginSessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUnitOfWork _uow;

  public async Task<LoginResult> LoginAsync(LoginUserCommand cmd, CancellationToken ct)
  {
    // 1. External authentication
    var provider = _providerFactory.Get(cmd.Provider);

    var external = await provider.AuthenticateAsync(
        cmd.Identifier,
        cmd.Secret,
        ct);

    // 2. Resolve identity
    var identity = await _identityRepo.GetByProviderAsync(
        external.Provider,
        external.ProviderUserId,
        ct);

    if (identity is null)
      throw new UnauthorizedAccessException("Identity not linked");

    // 3. Load user
    var user = await _userRepo.GetByIdAsync(identity.UserId, ct)
               ?? throw new InvalidOperationException("User not found");

    // 4. Session (delegated)
    var session = _sessionService.CreateSession(user, cmd.DeviceId);

    await _sessionService.PersistAsync(session, ct);

    // 5. Tokens
    var accessToken = _tokenService.CreateAccessToken(user, session);
    var refreshToken = _tokenService.CreateRefreshToken(user, session);

    session.SetRefreshToken(refreshToken);

    // 6. Commit
    await _uow.CommitAsync(ct);

    return new LoginResult(
        user.Id,
        accessToken,
        refreshToken,
        DateTime.UtcNow.AddMinutes(15));
  }
}