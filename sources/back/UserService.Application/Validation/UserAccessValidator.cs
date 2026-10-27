using UserService.Domain.Users;

namespace UserService.Application.Validation;

public sealed class AccountAccessDeniedException(string message) : Exception(message);

public sealed class UserAccessValidator : IUserAccessValidator
{
  public Task EnsureCanLoginAsync(User user, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    EnsureAllowed(user);
    return Task.CompletedTask;
  }

  public Task EnsureCanPlayAsync(User user, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    EnsureAllowed(user);
    return Task.CompletedTask;
  }

  private static void EnsureAllowed(User user)
  {
    ArgumentNullException.ThrowIfNull(user);

    switch (user.State)
    {
      case UserAccountState.Active:
        return;
      case UserAccountState.Suspended:
        throw new AccountAccessDeniedException("Account is suspended.");
      case UserAccountState.Banned:
        throw new AccountAccessDeniedException("Account is banned.");
      default:
        throw new InvalidOperationException($"Unsupported account state '{user.State}'.");
    }
  }
}