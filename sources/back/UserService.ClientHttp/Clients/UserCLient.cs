using UserService.Contracts.Commands.Users;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Users;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class UsersClient : HttpClientBase
{
  public UsersClient(HttpClient http) : base(http) { }

  public Task<UserDto> GetById(Guid id, CancellationToken ct)
      => GetAsync<UserDto>($"/api/users/{id}", ct);

  public Task<UserDto?> GetByUsername(string username, CancellationToken ct)
      => GetAsync<UserDto?>($"/api/users/username/{username}", ct);

  public Task<Guid> Create(CreateUserCommand command, CancellationToken ct)
      => PostAsync<CreateUserCommand, Guid>("/api/users", command, ct);

  public Task<bool> UpdateUsername(Guid id, UpdateUsernameCommand command, CancellationToken ct)
      => PostAsync<UpdateUsernameCommand, bool>(
          $"/api/users/{id}/username",
          command,
          ct);
}