using UserService.Domain.Users;

namespace UserService.Application.Validation;

public sealed class UserAccessValidator : IUserAccessValidator
{
  public Task EnsureCanLoginAsync(
      User user,
      CancellationToken cancellationToken)
  {
    EnsureAllowed(user);
    return Task.CompletedTask;
  }

  public Task EnsureCanPlayAsync(
      User user,
      CancellationToken cancellationToken)
  {
    EnsureAllowed(user);
    return Task.CompletedTask;
  }

  // =========================================================
  // CORE RULE ENGINE
  // =========================================================
  private static void EnsureAllowed(User user)
  {
    ArgumentNullException.ThrowIfNull(user);

    switch (user.State)
    {
      case UserAccountState.Active:
        return;

      case UserAccountState.Suspended:
        ThrowSuspended(user);
        break;

      case UserAccountState.Banned:
        ThrowBanned(user);
        break;

      default:
        throw new InvalidOperationException(
            $"Unsupported account state '{user.State}'.");
    }
  }

  // =========================================================
  // EXCEPTIONS
  // =========================================================
  private static void ThrowSuspended(User user)
  {
    if (user.SuspendedUntil.HasValue)
    {
      throw new UnauthorizedAccessException(
          $"Account suspended until {user.SuspendedUntil.Value:u}. Reason: {user.StatusReason}");
    }

    throw new UnauthorizedAccessException(
        $"Account suspended. Reason: {user.StatusReason}");
  }

  private static void ThrowBanned(User user)
  {
    throw new UnauthorizedAccessException(
        $"Account banned. Reason: {user.StatusReason}");
  }
}