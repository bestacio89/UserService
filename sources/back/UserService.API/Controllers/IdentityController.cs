using Microsoft.AspNetCore.Mvc;
using Franz.Common.Mediator.Dispatchers;
using UserService.Contracts.Commands.Identity;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/identity")]
public sealed class IdentityController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public IdentityController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpPost("guest")]
  public Task<Guid> CreateGuest(CreateGuestIdentityCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command, ct);

  [HttpPost("link")]
  public Task<Guid> Link(LinkUserIdentityCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command, ct);
}