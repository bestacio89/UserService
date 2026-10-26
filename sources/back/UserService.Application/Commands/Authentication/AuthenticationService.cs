using System.Security.Claims;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Pipelines.Core;
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

  public async Task<LoginResult> LoginAsync(LoginUserCommand cmd, CancellationToken ct)
  {
    ct.ThrowIfCancellationRequested();

    var external = await AuthenticateExternalAsync(cmd, ct);
    var identity = await ResolveIdentityAsync(external, ct);
    var user = await LoadUserAsync(identity, ct);
    await _userAccessValidator.EnsureCanLoginAsync(user, ct);

    var session = _sessionService.CreateSession(user, cmd.DeviceId);
    var accessToken = _tokenService.CreateAccessToken(user, session);
    var refreshToken = _tokenService.CreateRefreshToken(user, session);
    session.SetRefreshTokenHash(
        _tokenService.HashRefreshToken(refreshToken),
        _tokenService.GetRefreshTokenExpiry());

    await _sessionService.PersistAsync(session, ct);
    await _uow.CommitAsync(ct);

    return new LoginResult(user.Id, accessToken, refreshToken, _tokenService.GetAccessTokenExpiry());
  }

  public async Task<LoginResult> RefreshAsync(string refreshToken, CancellationToken ct)
  {
    ct.ThrowIfCancellationRequested();

    var principal = _tokenService.ValidateRefreshToken(refreshToken);
    var userId = ReadGuidClaim(principal, "uid");
    var sessionId = ReadGuidClaim(principal, "sid");
    var session = await _sessionService.GetByIdAsync(sessionId, ct)
        ?? throw new UnauthorizedAccessException("Invalid refresh token.");

    if (session.UserId != userId
        || !session.CanRefresh(_tokenService.HashRefreshToken(refreshToken), DateTime.UtcNow))
    {
      throw new UnauthorizedAccessException("Invalid refresh token.");
    }

    var user = await _userRepo.GetByIdAsync(userId, ct)
        ?? throw new UnauthorizedAccessException("Account is unavailable.");
    await _userAccessValidator.EnsureCanLoginAsync(user, ct);

    var newAccessToken = _tokenService.CreateAccessToken(user, session);
    var newRefreshToken = _tokenService.CreateRefreshToken(user, session);
    session.RotateRefreshTokenHash(
        _tokenService.HashRefreshToken(newRefreshToken),
        _tokenService.GetRefreshTokenExpiry());

    await _sessionService.UpdateAsync(session, ct);
    await _uow.CommitAsync(ct);

    return new LoginResult(user.Id, newAccessToken, newRefreshToken, _tokenService.GetAccessTokenExpiry());
  }

  public async Task LogoutAsync(string refreshToken, CancellationToken ct)
  {
    ct.ThrowIfCancellationRequested();

    var principal = _tokenService.ValidateRefreshToken(refreshToken, validateLifetime: false);
    var sessionId = ReadGuidClaim(principal, "sid");
    var session = await _sessionService.GetByIdAsync(sessionId, ct)
        ?? throw new UnauthorizedAccessException("Invalid refresh token.");

    if (!session.MatchesRefreshTokenHash(_tokenService.HashRefreshToken(refreshToken)))
      throw new UnauthorizedAccessException("Invalid refresh token.");

    session.Revoke();
    await _sessionService.UpdateAsync(session, ct);
    await _uow.CommitAsync(ct);
  }

  private async Task<ExternalIdentity> AuthenticateExternalAsync(LoginUserCommand cmd, CancellationToken ct)
  {
    var provider = _providerFactory.Get(cmd.Provider);
    return await provider.AuthenticateAsync(cmd.Identifier, cmd.Secret, ct);
  }

  private async Task<UserIdentity> ResolveIdentityAsync(ExternalIdentity external, CancellationToken ct)
  {
    var identity = await _identityRepo.GetByProviderAsync(external.Provider, external.ProviderUserId, ct);
    return identity ?? throw new UnauthorizedAccessException("Identity not linked.");
  }

  private async Task<User> LoadUserAsync(UserIdentity identity, CancellationToken ct)
      => await _userRepo.GetByIdAsync(identity.UserId, ct)
          ?? throw new UnauthorizedAccessException("Account is unavailable.");

  private static Guid ReadGuidClaim(ClaimsPrincipal principal, string claimType)
      => Guid.TryParse(principal.FindFirst(claimType)?.Value, out var value) && value != Guid.Empty
          ? value
          : throw new UnauthorizedAccessException("Invalid refresh token.");
}