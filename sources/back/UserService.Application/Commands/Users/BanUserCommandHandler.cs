using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.Commands.Admin;
using UserService.Domain.Users;

namespace UserService.Application.CommandHandlers.Admin;

public sealed class BanUserCommandHandler
    : ICommandHandler<BanUserCommand, bool>
{
  private readonly IEntityRepository<User,Guid> _userRepository;

  public BanUserCommandHandler(IEntityRepository<User, Guid> userRepository)
  {
    _userRepository = userRepository;
  }

  public async Task<bool> Handle(
      BanUserCommand command,
      CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(
        command.UserId,
        cancellationToken);

    if (user is null)
    {
      return false;
    }

    user.Ban(command.Reason);

    await _userRepository.UpdateAsync(user, cancellationToken);

    return true;
  }
}