using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    if (secret.Length < 32)
      throw new InvalidOperationException("JWT secret too weak (min 32 chars recommended)");

    _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    _accessLifetime = TimeSpan.FromMinutes(
        int.TryParse(config["Jwt:AccessTokenMinutes"], out var a) ? a : 15);

    _refreshLifetime = TimeSpan.FromDays(
        int.TryParse(config["Jwt:RefreshTokenDays"], out var r) ? r : 30);
  }

  // ------------------------
  // PUBLIC API
  // ------------------------

  public string CreateAccessToken(User user, UserSession session)
      => CreateToken(user, session, _accessLifetime, TokenType.Access);

  public string CreateRefreshToken(User user, UserSession session)
      => CreateToken(user, session, _refreshLifetime, TokenType.Refresh);

  public DateTime GetAccessTokenExpiry()
      => DateTime.UtcNow.Add(_accessLifetime);

  // ------------------------
  // CORE IMPLEMENTATION
  // ------------------------

  private string CreateToken(
      User user,
      UserSession session,
      TimeSpan lifetime,
      TokenType type)
  {
    var now = DateTime.UtcNow;

    var claims = new List<Claim>
        {
            new("uid", user.Id.ToString()),
            new("sid", session.Id.ToString()),
            new("typ", type.ToString().ToLowerInvariant()),
            new("jti", Guid.NewGuid().ToString())
        };

    // Device binding (critical for mobile + anti-abuse)
    if (!string.IsNullOrWhiteSpace(session.DeviceId))
    {
      claims.Add(new("did", session.DeviceId));
    }

    var credentials = new SigningCredentials(
        _signingKey,
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _issuer,
        audience: _audience,
        claims: claims,
        notBefore: now,
        expires: now.Add(lifetime),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  // ------------------------
  // INTERNAL MODEL
  // ------------------------

  private enum TokenType
  {
    Access,
    Refresh
  }
}