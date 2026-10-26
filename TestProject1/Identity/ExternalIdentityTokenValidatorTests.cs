using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Identity;
using UserService.Identity;

namespace UserService.Testing.Identity;

public sealed class ExternalIdentityTokenValidatorTests
{
  private const string Issuer = "https://accounts.google.com";
  private const string Audience = "test-google-client-id";

  [Fact]
  public async Task ValidateAsync_RejectsMalformedTokenBeforeMetadataRequest()
  {
    var validator = new ExternalIdentityTokenValidator(new ConfigurationBuilder().Build());

    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => validator.ValidateAsync(IdentityProvider.Google, "not-a-jwt", CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_FailsClosedWhenClientAudienceIsNotConfigured()
  {
    using var signingKey = RSA.Create(2048);
    var validator = CreateValidator(new ConfigurationBuilder().Build(), signingKey);

    var token = CreateToken(signingKey, Audience, Issuer, DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(5));

    await Assert.ThrowsAsync<InvalidOperationException>(
        () => validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_RejectsGuestAsExternalOidcProvider()
  {
    var validator = new ExternalIdentityTokenValidator(new ConfigurationBuilder().Build());
    const string token = "eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJ1c2VyIn0.c2lnbmF0dXJl";

    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
        () => validator.ValidateAsync(IdentityProvider.Guest, token, CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_AcceptsTokenWithValidSignatureIssuerAudienceAndLifetime()
  {
    using var signingKey = RSA.Create(2048);
    var validator = CreateValidator(CreateAudienceConfiguration(), signingKey);
    var token = CreateToken(signingKey, Audience, Issuer, DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(5));

    var principal = await validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None);

    Assert.Equal("provider-user-123", principal.FindFirst("sub")?.Value);
    Assert.Equal("player@example.test", principal.FindFirst("email")?.Value);
  }

  [Fact]
  public async Task ValidateAsync_RejectsWrongAudience()
  {
    using var signingKey = RSA.Create(2048);
    var validator = CreateValidator(CreateAudienceConfiguration(), signingKey);
    var token = CreateToken(signingKey, "different-client-id", Issuer, DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(5));

    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_RejectsWrongIssuer()
  {
    using var signingKey = RSA.Create(2048);
    var validator = CreateValidator(CreateAudienceConfiguration(), signingKey);
    var token = CreateToken(signingKey, Audience, "https://attacker.example", DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(5));

    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_RejectsExpiredToken()
  {
    using var signingKey = RSA.Create(2048);
    var validator = CreateValidator(CreateAudienceConfiguration(), signingKey);
    var token = CreateToken(signingKey, Audience, Issuer, DateTime.UtcNow.AddMinutes(-10),
        DateTime.UtcNow.AddMinutes(-5));

    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_RejectsTokenSignedByUntrustedKey()
  {
    using var trustedKey = RSA.Create(2048);
    using var attackerKey = RSA.Create(2048);
    var validator = CreateValidator(CreateAudienceConfiguration(), trustedKey);
    var token = CreateToken(attackerKey, Audience, Issuer, DateTime.UtcNow.AddMinutes(-1),
        DateTime.UtcNow.AddMinutes(5));

    await Assert.ThrowsAsync<UnauthorizedAccessException>(
        () => validator.ValidateAsync(IdentityProvider.Google, token, CancellationToken.None));
  }

  private static IConfiguration CreateAudienceConfiguration()
      => new ConfigurationBuilder()
          .AddInMemoryCollection(new Dictionary<string, string?>
          {
            ["Authentication:Google:ClientIds:0"] = Audience
          })
          .Build();

  private static ExternalIdentityTokenValidator CreateValidator(IConfiguration configuration, RSA rsa)
  {
    var signingKey = new RsaSecurityKey(rsa);
    var oidcConfiguration = new OpenIdConnectConfiguration { Issuer = Issuer };
    oidcConfiguration.SigningKeys.Add(signingKey);

    return new ExternalIdentityTokenValidator(
        configuration,
        _ => new TestConfigurationManager(oidcConfiguration));
  }

  private static string CreateToken(
      RSA rsa,
      string audience,
      string issuer,
      DateTime notBefore,
      DateTime expires)
  {
    var key = new RsaSecurityKey(rsa);
    var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
    var token = new JwtSecurityToken(
        issuer,
        audience,
        [
          new Claim("sub", "provider-user-123"),
          new Claim("email", "player@example.test"),
          new Claim("email_verified", "true")
        ],
        notBefore,
        expires,
        credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  private sealed class TestConfigurationManager(OpenIdConnectConfiguration configuration)
      : IConfigurationManager<OpenIdConnectConfiguration>
  {
    public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        => Task.FromResult(configuration);

    public void RequestRefresh()
    {
    }
  }
}
