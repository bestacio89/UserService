using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using UserService.Contracts.Commands.Admin;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Admin;
using UserService.Domain.Users;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
public sealed class AdminUsersController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public AdminUsersController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // GET admin profile
  [HttpGet("{id:guid}/profile")]
  public Task<UserAdminProfileDto> GetAdminProfile(Guid id, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserAdminProfileQuery(id), ct);

  // GET account state
  [HttpGet("{id:guid}/state")]
  public Task<UserAccountState?> GetState(Guid id, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserAccountStateQuery(id), ct);

  // BAN
  [HttpPost("{id:guid}/ban")]
  public Task<bool> Ban(Guid id, BanUserCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = id }, ct);

  // SUSPEND
  [HttpPost("{id:guid}/suspend")]
  public Task<bool> Suspend(Guid id, SuspendUserCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = id }, ct);

  // REACTIVATE
  [HttpPost("{id:guid}/reactivate")]
  public Task<bool> Reactivate(Guid id, CancellationToken ct)
      => _dispatcher.SendAsync(new ReactivateUserCommand(id), ct);
}