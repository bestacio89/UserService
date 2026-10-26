using System.Security.Cryptography;
using System.Text;
using Franz.Common.Business.Domain;

namespace UserService.Domain.Authentication;

public sealed class UserSession : Entity<Guid>
{
  public Guid UserId { get; private set; }
  public string? DeviceId { get; private set; }
  public string RefreshTokenHash { get; private set; } = null!;
  public DateTime CreatedAt { get; private set; }
  public DateTime RefreshTokenExpiresAt { get; private set; }
  public DateTime? RevokedAt { get; private set; }
  public DateTime? LastRotatedAt { get; private set; }

  public bool IsRevoked => RevokedAt.HasValue;

  private UserSession() { }

  public void Define(Guid userId, string? deviceId)
  {
    if (userId == Guid.Empty)
      throw new ArgumentException("A valid user ID is required.", nameof(userId));

    UserId = userId;
    DeviceId = string.IsNullOrWhiteSpace(deviceId) ? null : deviceId.Trim();
    CreatedAt = DateTime.UtcNow;
    RefreshTokenExpiresAt = DateTime.MinValue;
    MarkCreated("system");
  }

  public void SetRefreshTokenHash(string tokenHash, DateTime expiresAt)
  {
    if (RevokedAt.HasValue)
      throw new InvalidOperationException("A revoked session cannot receive a refresh token.");

    ValidateRefreshToken(tokenHash, expiresAt);
    RefreshTokenHash = tokenHash;
    RefreshTokenExpiresAt = expiresAt;
  }

  public void RotateRefreshTokenHash(string tokenHash, DateTime expiresAt)
  {
    if (RevokedAt.HasValue)
      throw new InvalidOperationException("A revoked session cannot be rotated.");

    ValidateRefreshToken(tokenHash, expiresAt);
    RefreshTokenHash = tokenHash;
    RefreshTokenExpiresAt = expiresAt;
    LastRotatedAt = DateTime.UtcNow;
    MarkUpdated("system");
  }

  public bool MatchesRefreshTokenHash(string tokenHash)
  {
    if (string.IsNullOrWhiteSpace(RefreshTokenHash) || string.IsNullOrWhiteSpace(tokenHash))
      return false;

    var stored = Encoding.UTF8.GetBytes(RefreshTokenHash);
    var supplied = Encoding.UTF8.GetBytes(tokenHash);
    return stored.Length == supplied.Length
        && CryptographicOperations.FixedTimeEquals(stored, supplied);
  }

  public bool CanRefresh(string tokenHash, DateTime now)
      => !IsRevoked
          && RefreshTokenExpiresAt > now
          && MatchesRefreshTokenHash(tokenHash);

  public void Revoke()
  {
    if (RevokedAt.HasValue)
      return;

    RevokedAt = DateTime.UtcNow;
    MarkUpdated("system");
  }

  private static void ValidateRefreshToken(string tokenHash, DateTime expiresAt)
  {
    if (string.IsNullOrWhiteSpace(tokenHash))
      throw new ArgumentException("A refresh-token hash is required.", nameof(tokenHash));

    if (expiresAt.Kind != DateTimeKind.Utc || expiresAt <= DateTime.UtcNow)
      throw new ArgumentOutOfRangeException(nameof(expiresAt), "Refresh-token expiry must be a future UTC timestamp.");
  }
}