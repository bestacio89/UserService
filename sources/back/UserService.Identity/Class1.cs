using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Domain.Identity;

namespace UserService.Identity.Guest;

public sealed class GuestAuthenticationProvider : IAuthenticationProvider
{
  public IdentityProvider Provider => IdentityProvider.Guest;

  public Task<ExternalIdentity> AuthenticateAsync(
      string identifier,
      string secret,
      CancellationToken ct)
  {
    return Task.FromResult(new ExternalIdentity(
        Provider: IdentityProvider.Guest,
        ProviderUserId: Guid.NewGuid().ToString(),
        Email: null,
        EmailVerified: false));
  }
}