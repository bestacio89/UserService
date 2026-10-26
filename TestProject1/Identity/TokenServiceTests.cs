using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Commands.Authentication.Services;

namespace UserService.Testing.Identity;

public sealed class TokenServiceTests
{
  private const string Issuer = "UserService.Tests";
  private const string Audience = "UserService.Tests.Client";
  private const string Secret = "unit-test-only-signing-key-never-use-in-production";

  [Fact]
  public void ValidateRefreshToken_AcceptsSignedRefreshToken()
  {
    var service = CreateService();
    var token = CreateToken("refresh", DateTime.UtcNow.AddMinutes(5));
    var principal = service.ValidateRefreshToken(token);

    Assert.Equal("refresh", principal.FindFirst("typ")?.Value);
    Assert.Equal(64, service.HashRefreshToken(token).Length);
  }

  [Fact]
  public void ValidateRefreshToken_RejectsAccessToken()
  {
    var service = CreateService();
    var token = CreateToken("access", DateTime.UtcNow.AddMinutes(5));

    Assert.Throws<UnauthorizedAccessException>(() => service.ValidateRefreshToken(token));
  }

  [Fact]
  public void ValidateRefreshToken_RejectsExpiredToken()
  {
    var service = CreateService();
    var token = CreateToken("refresh", DateTime.UtcNow.AddMinutes(-5));

    Assert.Throws<UnauthorizedAccessException>(() => service.ValidateRefreshToken(token));
  }

  [Fact]
  public void HashRefreshToken_IsDeterministicAndDoesNotExposeTheToken()
  {
    var service = CreateService();
    const string token = "refresh-token-test-value";

    var first = service.HashRefreshToken(token);
    var second = service.HashRefreshToken(token);

    Assert.Equal(first, second);
    Assert.Equal(64, first.Length);
    Assert.DoesNotContain(token, first, StringComparison.Ordinal);
  }

  private static TokenService CreateService()
      => new(new ConfigurationBuilder()
          .AddInMemoryCollection(new Dictionary<string, string?>
          {
            ["Jwt:Issuer"] = Issuer,
            ["Jwt:Audience"] = Audience,
            ["Jwt:Secret"] = Secret
          })
          .Build());

  private static string CreateToken(string tokenType, DateTime expiresAt)
  {
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: Issuer,
        audience: Audience,
        claims:
        [
          new Claim("uid", Guid.NewGuid().ToString()),
          new Claim("sid", Guid.NewGuid().ToString()),
          new Claim("typ", tokenType),
          new Claim("jti", Guid.NewGuid().ToString())
        ],
        notBefore: expiresAt.AddMinutes(-10),
        expires: expiresAt,
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}