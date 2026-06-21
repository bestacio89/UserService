using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using UserService.Application.Commands.Authentication.Services;
using UserService.Application.Validation;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Contracts.Persistence;
using UserService.Domain.Authentication;
using UserService.Domain.Identity;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
  private readonly IAuthenticationProviderFactory _providerFactory;
  private readonly IUserIdentityLookupRepository _identityRepo;
  private readonly IEntityRepository<User, Guid> _userRepo;
  private readonly ILoginSessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserAccessValidator _userAccessValidator;
  private readonly IUnitOfWork _uow;

  public AuthenticationService(
      IAuthenticationProviderFactory providerFactory,
      IUserIdentityLookupRepository identityRepo,
      IEntityRepository<User, Guid> userRepo,
      ILoginSessionService sessionService,
      ITokenService tokenService,
      IUserAccessValidator userAccessValidator,
      IUnitOfWork uow)
  {
    _providerFactory = providerFactory;
    _identityRepo = identityRepo;
    _userRepo = userRepo;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userAccessValidator = userAccessValidator;
    _uow = uow;
  }

  public async Task<LoginResult> LoginAsync(
      LoginUserCommand cmd,
      CancellationToken ct)
  {
    var external = await AuthenticateExternalAsync(cmd, ct);

    var identity = await ResolveIdentityAsync(external, ct);

    var user = await LoadUserAsync(identity, ct);

    ValidateUserAccess(user, ct);

    var session = CreateSession(user, cmd);

    var tokens = GenerateTokens(user, session);

    session.SetRefreshToken(tokens.RefreshToken);

    await PersistAsync(session, ct);

    return BuildResult(user, tokens);
  }

  // =========================================================
  // STEP 1 — External authentication
  // =========================================================

  private async Task<ExternalIdentity> AuthenticateExternalAsync(
      LoginUserCommand cmd,
      CancellationToken ct)
  {
    var provider = _providerFactory.Get(cmd.Provider);

    return await provider.AuthenticateAsync(
        cmd.Identifier,
        cmd.Secret,
        ct);
  }

  // =========================================================
  // STEP 2 — Identity resolution
  // =========================================================

  private async Task<UserIdentity> ResolveIdentityAsync(
      ExternalIdentity external,
      CancellationToken ct)
  {
    var identity = await _identityRepo.GetByProviderAsync(
        external.Provider,
        external.ProviderUserId,
        ct);

    return identity
        ?? throw new UnauthorizedAccessException("Identity not linked");
  }

  // =========================================================
  // STEP 3 — Load user aggregate
  // =========================================================

  private async Task<User> LoadUserAsync(
      UserIdentity identity,
      CancellationToken ct)
  {
    return await _userRepo.GetByIdAsync(identity.UserId, ct)
           ?? throw new InvalidOperationException("User not found");
  }

  // =========================================================
  // STEP 4 — Account access validation
  // =========================================================

  private void ValidateUserAccess(User user, CancellationToken cancellation)
  {
    _userAccessValidator.EnsureCanLoginAsync(user, cancellation);
  }

  // =========================================================
  // STEP 5 — Session creation
  // =========================================================

  private UserSession CreateSession(
      User user,
      LoginUserCommand cmd)
  {
    return _sessionService.CreateSession(
        user,
        cmd.DeviceId);
  }

  // =========================================================
  // STEP 6 — Token generation
  // =========================================================

  private (string AccessToken, string RefreshToken) GenerateTokens(
      User user,
      UserSession session)
  {
    return (
        _tokenService.CreateAccessToken(user, session),
        _tokenService.CreateRefreshToken(user, session)
    );
  }

  // =========================================================
  // STEP 7 — Persistence
  // =========================================================

  private async Task PersistAsync(
      UserSession session,
      CancellationToken ct)
  {
    await _sessionService.PersistAsync(session, ct);

    await _uow.CommitAsync(ct);
  }

  // =========================================================
  // RESULT
  // =========================================================

  private static LoginResult BuildResult(
      User user,
      (string AccessToken, string RefreshToken) tokens)
  {
    return new LoginResult(
        user.Id,
        tokens.AccessToken,
        tokens.RefreshToken,
        DateTime.UtcNow.AddMinutes(15));
  }
}