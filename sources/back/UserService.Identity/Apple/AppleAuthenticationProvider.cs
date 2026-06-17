using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserService.Contracts.DTOs.Authentication;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Domain.Identity;

namespace UserService.Identity.Apple;

public sealed class AppleAuthenticationProvider : IAuthenticationProvider
{
  public IdentityProvider Provider => IdentityProvider.Apple;

  public Task<ExternalIdentity> AuthenticateAsync(
      string identifier,
      string secret,
      CancellationToken ct)
  {
    var handler = new JwtSecurityTokenHandler();

    if (!handler.CanReadToken(secret))
      throw new UnauthorizedAccessException("Invalid Apple identity token");

    var jwt = handler.ReadJwtToken(secret);

    var providerUserId =
        jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

    var email =
        jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

    var emailVerified =
        jwt.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value == "true";

    if (string.IsNullOrWhiteSpace(providerUserId))
      throw new UnauthorizedAccessException("Apple token missing subject");

    return Task.FromResult(new ExternalIdentity(
        Provider: IdentityProvider.Apple,
        ProviderUserId: providerUserId,
        Email: email,
        EmailVerified: emailVerified
    ));
  }
}