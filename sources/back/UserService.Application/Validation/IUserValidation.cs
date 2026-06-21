using UserService.Domain.Users;

namespace UserService.Application.Validation;

public interface IUserAccessValidator
{
  Task EnsureCanLoginAsync(
      User user,
      CancellationToken cancellationToken);

  Task EnsureCanPlayAsync(
      User user,
      CancellationToken cancellationToken);
}