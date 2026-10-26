using Microsoft.Extensions.Configuration;
using UserService.Domain.Identity;
using UserService.Identity;

namespace UserService.Testing.Identity;

public sealed class ExternalIdentityTokenValidatorTests
{
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
    var validator = new ExternalIdentityTokenValidator(new ConfigurationBuilder().Build());
    const string syntacticallyReadableToken = "eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJ1c2VyIn0.c2lnbmF0dXJl";

    await Assert.ThrowsAsync<InvalidOperationException>(
        () => validator.ValidateAsync(
            IdentityProvider.Google,
            syntacticallyReadableToken,
            CancellationToken.None));
  }

  [Fact]
  public async Task ValidateAsync_RejectsGuestAsExternalOidcProvider()
  {
    var validator = new ExternalIdentityTokenValidator(new ConfigurationBuilder().Build());

    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
        () => validator.ValidateAsync(
            IdentityProvider.Guest,
            "eyJhbGciOiJSUzI1NiJ9.eyJzdWIiOiJ1c2VyIn0.c2lnbmF0dXJl",
            CancellationToken.None));
  }
}
