using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using UserService.Contracts.Commands.Users;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Users;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public UsersController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // GET /api/users/{id}
  [HttpGet("{id:guid}")]
  public Task<UserDto> GetById(Guid id, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserByIdQuery(id), ct);

  // GET /api/users/username/{username}
  [HttpGet("username/{username}")]
  public Task<UserDto?> GetByUsername(string username, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserByUsernameQuery(username), ct);

  // POST /api/users
  [HttpPost]
  public Task<Guid> Create(CreateUserCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command, ct);

  // PUT /api/users/{id}/username
  [HttpPut("{id:guid}/username")]
  public Task<bool> UpdateUsername(Guid id, UpdateUsernameCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = id }, ct);
}