using Franz.Common.Business.Domain;

namespace UserService.Domain.Authentication;

public sealed class UserSession : Entity<Guid>
{
  public Guid UserId { get; private set; }
  public string? DeviceId { get; private set; }
  public string RefreshToken { get; private set; }
  public DateTime CreatedAt { get; private set; }

  private UserSession() { }

  public void Define(Guid userId, string? deviceId)
  {
    UserId = userId;
    DeviceId = deviceId;
    CreatedAt = DateTime.UtcNow;

    MarkCreated("system");
  }

  public void SetRefreshToken(string token)
  {
    if (string.IsNullOrWhiteSpace(token))
      throw new ArgumentException("Refresh token cannot be empty");

    RefreshToken = token;
  }
}