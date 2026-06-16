using Franz.Common.Mediator.Context;
using UserService.Contracts.Commands.Users;


namespace UserService.Application.Commands.Users.Services;

public interface IUserCreationService
{
  Task<Guid> CreateAsync(
      CreateUserCommand request,
      CancellationToken cancellationToken);
}