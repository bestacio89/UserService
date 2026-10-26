using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Identity;

namespace UserService.Identity;

public sealed class ExternalIdentityTokenValidator
{
  private const string GoogleIssuer = "https://accounts.google.com";
  private const string AppleIssuer = "https://appleid.apple.com";

  private readonly IConfiguration _configuration;
  private readonly Func<string, IConfigurationManager<OpenIdConnectConfiguration>> _configurationManagerFactory;
  private readonly ConcurrentDictionary<string, IConfigurationManager<OpenIdConnectConfiguration>>
      _configurationManagers = new(StringComparer.Ordinal);

  public ExternalIdentityTokenValidator(IConfiguration configuration)
      : this(configuration, CreateConfigurationManager)
  {
  }

  public ExternalIdentityTokenValidator(
      IConfiguration configuration,
      Func<string, IConfigurationManager<OpenIdConnectConfiguration>> configurationManagerFactory)
  {
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    _configurationManagerFactory = configurationManagerFactory
        ?? throw new ArgumentNullException(nameof(configurationManagerFactory));
  }

  public async Task<ClaimsPrincipal> ValidateAsync(
      IdentityProvider provider, string token, CancellationToken cancellationToken)
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
      throw new InvalidOperationException($"No client audience is configured for external provider '{provider}'.");

    var manager = _configurationManagers.GetOrAdd(metadataAddress, _configurationManagerFactory);
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

  private static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(string address)
      => new ConfigurationManager<OpenIdConnectConfiguration>(
          address,
          new OpenIdConnectConfigurationRetriever(),
          new HttpDocumentRetriever { RequireHttps = true });

  private static (string MetadataAddress, string[] Issuers, string[] AudienceKeys)
      GetProviderSettings(IdentityProvider provider)
      => provider switch
      {
        IdentityProvider.Google =>
            ("https://accounts.google.com/.well-known/openid-configuration",
             [GoogleIssuer, "accounts.google.com"], ["Authentication:Google:ClientIds"]),
        IdentityProvider.Apple =>
            ("https://appleid.apple.com/.well-known/openid-configuration",
             [AppleIssuer], ["Authentication:Apple:ClientIds"]),
        _ => throw new ArgumentOutOfRangeException(
            nameof(provider), provider, "Only Google and Apple use external ID-token validation.")
      };
}
