using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using UserService.Application.Commands.Users.Services;
using UserService.Contracts.Commands.Users;

namespace UserService.Application.Commands.Users;

public sealed class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
  private readonly IUserCreationService _userCreationService;

  public CreateUserCommandHandler(IUserCreationService userCreationService)
  {
    _userCreationService = userCreationService;
  }

  public async Task<Guid> Handle(
      CreateUserCommand command,
      CancellationToken cancellationToken)
  {
    return await _userCreationService.CreateAsync(
        command,
        cancellationToken
    );
  }
}