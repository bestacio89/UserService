using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using UserService.Application.Commands.Identity.Services;
using UserService.Contracts.Commands.Identity;

namespace UserService.Application.Commands.Identity.Handlers;

public sealed class LinkUserIdentityCommandHandler
    : ICommandHandler<LinkUserIdentityCommand, Guid>
{
  private readonly IUserIdentityService _service;

  public LinkUserIdentityCommandHandler(IUserIdentityService service)
  {
    _service = service;
  }

  public async Task<Guid> Handle(
      LinkUserIdentityCommand command,
      CancellationToken cancellationToken)
  {
    return await _service.LinkAsync(command, cancellationToken);
  }
}