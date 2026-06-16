using Franz.Common.Business.Domain;

namespace UserService.Domain.Identity;

public sealed class UserIdentity : Entity<Guid>
{
  public Guid UserId { get; private set; }

  public IdentityProvider Provider { get; private set; }

  public string ExternalId { get; private set; } = null!;

  public string? DeviceId { get; private set; }

  private UserIdentity() { }

  public void Define(
      Guid userId,
      IdentityProvider provider,
      string externalId,
      string? deviceId)
  {
    UserId = userId;
    Provider = provider;
    ExternalId = externalId;
    DeviceId = deviceId;
    MarkCreated("system");
  }
}

