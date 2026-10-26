using System.Security.Claims;
using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Domain.Identity;

namespace UserService.Identity.Google;

public sealed class GoogleAuthenticationProvider : IAuthenticationProvider
{
  private readonly ExternalIdentityTokenValidator _tokenValidator;

  public GoogleAuthenticationProvider(ExternalIdentityTokenValidator tokenValidator)
  {
    _tokenValidator = tokenValidator;
  }

  public IdentityProvider Provider => IdentityProvider.Google;

  public async Task<ExternalIdentity> AuthenticateAsync(
      string identifier,
      string secret,
      CancellationToken ct)
  {
    var principal = await _tokenValidator.ValidateAsync(Provider, secret, ct);
    var providerUserId = principal.FindFirstValue("sub");

    if (string.IsNullOrWhiteSpace(providerUserId))
      throw new UnauthorizedAccessException("Invalid external identity token.");

    var email = principal.FindFirstValue("email");
    var emailVerified = bool.TryParse(principal.FindFirstValue("email_verified"), out var verified)
        && verified;

    return new ExternalIdentity(
        Provider: Provider,
        ProviderUserId: providerUserId,
        Email: email,
        EmailVerified: emailVerified);
  }
}
