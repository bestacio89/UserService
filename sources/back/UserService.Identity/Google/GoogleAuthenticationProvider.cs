using System.IdentityModel.Tokens.Jwt;
using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Domain.Identity;

namespace UserService.Identity.Google;

public sealed class GoogleAuthenticationProvider : IAuthenticationProvider
{
  public IdentityProvider Provider => IdentityProvider.Google;

  public Task<ExternalIdentity> AuthenticateAsync(
      string identifier,
      string secret,
      CancellationToken ct)
  {
    // secret = Google ID Token (JWT)

    var handler = new JwtSecurityTokenHandler();

    if (!handler.CanReadToken(secret))
      throw new UnauthorizedAccessException("Invalid Google token");

    var jwt = handler.ReadJwtToken(secret);

    var providerUserId =
        jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

    var email =
        jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

    var emailVerifiedStr =
        jwt.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value;

    if (string.IsNullOrWhiteSpace(providerUserId))
      throw new UnauthorizedAccessException("Google token missing subject");

    return Task.FromResult(new ExternalIdentity(
        Provider: IdentityProvider.Google,
        ProviderUserId: providerUserId,
        Email: email,
        EmailVerified: emailVerifiedStr == "true"
    ));
  }
}