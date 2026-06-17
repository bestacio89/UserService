using Franz.Common.Business.Domain;
using UserService.Domain.Authentication;

namespace UserService.Domain.Users;

public sealed class User : Entity<Guid>
{
  public string Username { get; private set; } = null!;

  public UserAccountState State { get; private set; }

  public string? StatusReason { get; private set; }

  public DateTime? SuspendedUntil { get; private set; }

  private User() { }

  public void Define(string username)
  {
    Username = username;

    State = UserAccountState.Active;
    StatusReason = null;
    SuspendedUntil = null;

    MarkCreated("system");
  }

  public void Ban(string reason)
  {
    if (string.IsNullOrWhiteSpace(reason))
    {
      throw new ArgumentException(
          "A ban reason is required.",
          nameof(reason));
    }

    State = UserAccountState.Banned;
    StatusReason = reason;
    SuspendedUntil = null;

    MarkUpdated("system");
  }

  public void Suspend(
      string reason,
      DateTime? until)
  {
    if (string.IsNullOrWhiteSpace(reason))
    {
      throw new ArgumentException(
          "A suspension reason is required.",
          nameof(reason));
    }

    State = UserAccountState.Suspended;
    StatusReason = reason;
    SuspendedUntil = until;

    MarkUpdated("system");
  }

  public void Reactivate()
  {
    State = UserAccountState.Active;
    StatusReason = null;
    SuspendedUntil = null;

    MarkUpdated("system");
  }

  public void ChangeUsername(string username)
  {
    if (string.IsNullOrWhiteSpace(username))
    {
      throw new ArgumentException(
          "Username cannot be empty.",
          nameof(username));
    }

    Username = username;

    MarkUpdated("system");
  }

}