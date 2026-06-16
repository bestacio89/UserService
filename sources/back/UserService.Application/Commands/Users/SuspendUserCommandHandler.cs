using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.Commands.Admin;
using UserService.Domain.Users;

public sealed class SuspendUserCommandHandler
    : ICommandHandler<SuspendUserCommand, bool>
{
  private readonly IEntityRepository<User, Guid> _userRepository;

  public SuspendUserCommandHandler(IEntityRepository<User, Guid> userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<bool> Handle(
      SuspendUserCommand command,
      CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(
        command.UserId,
        cancellationToken);

    if (user is null)
    {
      return false;
    }

    user.Suspend(
        command.Reason,
        command.Until);

    await _userRepository.UpdateAsync(user, cancellationToken);

    return true;
  }
}