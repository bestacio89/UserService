using UserService.Contracts.Commands.Identity;

namespace UserService.Application.Commands.Identity.Services;

public interface IUserIdentityService
{
  Task<Guid> LinkAsync(
      LinkUserIdentityCommand command,
      CancellationToken cancellationToken);
}