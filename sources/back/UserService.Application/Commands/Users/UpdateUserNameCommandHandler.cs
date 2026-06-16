using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.Commands.Users;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Users;

public sealed class UpdateUsernameCommandHandler
    : ICommandHandler<UpdateUsernameCommand, bool>
{
  private readonly IEntityRepository<User, Guid> _userRepository;

  public UpdateUsernameCommandHandler( IEntityRepository<User, Guid> userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<bool> Handle(
      UpdateUsernameCommand command,
      CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(
        command.UserId,
        cancellationToken);

    if (user is null)
    {
      return false;
    }

    user.ChangeUsername(command.NewUsername);

    await _userRepository.UpdateAsync(user, cancellationToken);

    return true;
  }
}