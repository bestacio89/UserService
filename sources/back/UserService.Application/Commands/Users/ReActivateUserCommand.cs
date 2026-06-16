using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.Commands.Admin;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Users;

public sealed class ReactivateUserCommandHandler
    : ICommandHandler<ReactivateUserCommand, bool>
{
  private readonly IEntityRepository<User, Guid> _userRepository;

  public ReactivateUserCommandHandler(IEntityRepository<User, Guid> userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<bool> Handle(
      ReactivateUserCommand command,
      CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(
        command.UserId,
        cancellationToken);

    if (user is null)
    {
      return false;
    }

    user.Reactivate();

    await _userRepository.UpdateAsync(user, cancellationToken);

    return true;
  }
}