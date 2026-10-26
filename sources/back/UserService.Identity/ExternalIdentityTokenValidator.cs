using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Identity;

namespace UserService.Identity;

/// <summary>
/// Validates external OpenID Connect ID tokens before any claims are trusted.
/// Provider subjects are accepted only after signature, issuer, audience, and lifetime validation.
/// </summary>
public sealed class ExternalIdentityTokenValidator
{
  private const string GoogleIssuer = "https://accounts.google.com";
  private const string AppleIssuer = "https://appleid.apple.com";

  private static readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>>
      ConfigurationManagers = new(StringComparer.Ordinal);

  private readonly IConfiguration _configuration;

  public ExternalIdentityTokenValidator(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public async Task<ClaimsPrincipal> ValidateAsync(
      IdentityProvider provider,
      string token,
      CancellationToken cancellationToken)
  {
    var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
    if (string.IsNullOrWhiteSpace(token) || !handler.CanReadToken(token))
      throw new UnauthorizedAccessException("Invalid external identity token.");

    var (metadataAddress, issuers, audienceKeys) = GetProviderSettings(provider);
    var validAudiences = audienceKeys
        .SelectMany(key => _configuration.GetSection(key).GetChildren().Select(child => child.Value))
        .Append(_configuration[$"Authentication:{provider}:ClientId"])
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value!)
        .Distinct(StringComparer.Ordinal)
        .ToArray();

    if (validAudiences.Length == 0)
      throw new InvalidOperationException(
          $"No client audience is configured for external provider '{provider}'.");

    var manager = ConfigurationManagers.GetOrAdd(
        metadataAddress,
        address => new ConfigurationManager<OpenIdConnectConfiguration>(
            address,
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever { RequireHttps = true }));

    var oidcConfiguration = await manager.GetConfigurationAsync(cancellationToken);
    var validationParameters = new TokenValidationParameters
    {
      RequireSignedTokens = true,
      ValidateIssuerSigningKey = true,
      IssuerSigningKeys = oidcConfiguration.SigningKeys,
      ValidateIssuer = true,
      ValidIssuers = issuers,
      ValidateAudience = true,
      ValidAudiences = validAudiences,
      ValidateLifetime = true,
      RequireExpirationTime = true,
      ClockSkew = TimeSpan.FromMinutes(2),
      ValidAlgorithms = [SecurityAlgorithms.RsaSha256]
    };

    try
    {
      return handler.ValidateToken(token, validationParameters, out _);
    }
    catch (SecurityTokenSignatureKeyNotFoundException)
    {
      // Ask the OIDC metadata manager to refresh its signing keys and retry once.
      manager.RequestRefresh();
      oidcConfiguration = await manager.GetConfigurationAsync(cancellationToken);
      validationParameters.IssuerSigningKeys = oidcConfiguration.SigningKeys;

      try
      {
        return handler.ValidateToken(token, validationParameters, out _);
      }
      catch (SecurityTokenException)
      {
        throw new UnauthorizedAccessException("Invalid external identity token.");
      }
    }
    catch (SecurityTokenException)
    {
      throw new UnauthorizedAccessException("Invalid external identity token.");
    }
  }

  private static (string MetadataAddress, string[] Issuers, string[] AudienceKeys)
      GetProviderSettings(IdentityProvider provider)
      => provider switch
      {
        IdentityProvider.Google =>
            ("https://accounts.google.com/.well-known/openid-configuration",
             [GoogleIssuer, "accounts.google.com"],
             ["Authentication:Google:ClientIds"]),
        IdentityProvider.Apple =>
            ("https://appleid.apple.com/.well-known/openid-configuration",
             [AppleIssuer],
             ["Authentication:Apple:ClientIds"]),
        _ => throw new ArgumentOutOfRangeException(
            nameof(provider), provider, "Only Google and Apple use external ID-token validation.")
      };
}
