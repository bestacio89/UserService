using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Domain.Authentication;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication.Services;

public sealed class TokenService : ITokenService
{
  private readonly string _issuer;
  private readonly string _audience;
  private readonly SymmetricSecurityKey _signingKey;
  private readonly TimeSpan _accessLifetime;
  private readonly TimeSpan _refreshLifetime;

  public TokenService(IConfiguration config)
  {
    _issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing Jwt:Issuer");
    _audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing Jwt:Audience");

    var secret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Missing Jwt:Secret");
    if (Encoding.UTF8.GetByteCount(secret) < 32)
      throw new InvalidOperationException("JWT secret must contain at least 32 bytes.");

    _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    _accessLifetime = ReadPositiveLifetime(config["Jwt:AccessTokenMinutes"], 15, TimeSpan.FromDays(1), "access-token lifetime");
    _refreshLifetime = ReadPositiveLifetime(config["Jwt:RefreshTokenDays"], 30, TimeSpan.FromDays(365), "refresh-token lifetime");
  }

  public string CreateAccessToken(User user, UserSession session)
      => CreateToken(user, session, _accessLifetime, TokenType.Access);

  public string CreateRefreshToken(User user, UserSession session)
      => CreateToken(user, session, _refreshLifetime, TokenType.Refresh);

  public DateTime GetAccessTokenExpiry()
      => DateTime.UtcNow.Add(_accessLifetime);

  public DateTime GetRefreshTokenExpiry()
      => DateTime.UtcNow.Add(_refreshLifetime);

  public string HashRefreshToken(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
      throw new ArgumentException("A refresh token is required.", nameof(token));

    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
  }

  public ClaimsPrincipal ValidateRefreshToken(string token, bool validateLifetime = true)
  {
    if (string.IsNullOrWhiteSpace(token))
      throw new UnauthorizedAccessException("Invalid refresh token.");

    var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
    try
    {
      var principal = handler.ValidateToken(token, new TokenValidationParameters
      {
        RequireSignedTokens = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = _signingKey,
        ValidateIssuer = true,
        ValidIssuer = _issuer,
        ValidateAudience = true,
        ValidAudience = _audience,
        ValidateLifetime = validateLifetime,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
      }, out _);

      if (!string.Equals(principal.FindFirst("typ")?.Value, "refresh", StringComparison.Ordinal)
          || !Guid.TryParse(principal.FindFirst("uid")?.Value, out _)
          || !Guid.TryParse(principal.FindFirst("sid")?.Value, out _))
      {
        throw new UnauthorizedAccessException("Invalid refresh token.");
      }

      return principal;
    }
    catch (SecurityTokenException)
    {
      throw new UnauthorizedAccessException("Invalid refresh token.");
    }
    catch (ArgumentException)
    {
      throw new UnauthorizedAccessException("Invalid refresh token.");
    }
  }

  private string CreateToken(User user, UserSession session, TimeSpan lifetime, TokenType type)
  {
    ArgumentNullException.ThrowIfNull(user);
    ArgumentNullException.ThrowIfNull(session);

    var now = DateTime.UtcNow;
    var claims = new List<Claim>
    {
      new("uid", user.Id.ToString()),
      new("sid", session.Id.ToString()),
      new("typ", type.ToString().ToLowerInvariant()),
      new("jti", Guid.NewGuid().ToString())
    };

    if (!string.IsNullOrWhiteSpace(session.DeviceId))
      claims.Add(new Claim("did", session.DeviceId));

    var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
      issuer: _issuer,
      audience: _audience,
      claims: claims,
      notBefore: now,
      expires: now.Add(lifetime),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  private static TimeSpan ReadPositiveLifetime(string? configured, int fallback, TimeSpan maximum, string name)
  {
    if (configured is null)
      return TimeSpan.FromMinutes(fallback).CompareTo(maximum) <= 0
          ? TimeSpan.FromMinutes(fallback)
          : maximum;

    if (!int.TryParse(configured, out var value) || value <= 0)
      throw new InvalidOperationException($"The configured {name} must be a positive integer.");

    var lifetime = name.StartsWith("refresh", StringComparison.Ordinal)
        ? TimeSpan.FromDays(value)
        : TimeSpan.FromMinutes(value);

    if (lifetime > maximum)
      throw new InvalidOperationException($"The configured {name} exceeds the supported maximum.");

    return lifetime;
  }

  private enum TokenType
  {
    Access,
    Refresh
  }
}