using UserService.Contracts.Commands.Admin;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Admin;
using UserService.Domain.Users;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class AdminUsersClient : HttpClientBase
{
  public AdminUsersClient(HttpClient http) : base(http) { }

  public Task<UserAdminProfileDto> GetProfile(Guid userId, CancellationToken ct)
      => GetAsync<UserAdminProfileDto>($"/api/admin/users/{userId}/profile", ct);

  public Task<UserAccountState?> GetState(Guid userId, CancellationToken ct)
      => GetAsync<UserAccountState?>($"/api/admin/users/{userId}/state", ct);

  public Task<bool> Ban(Guid userId, BanUserCommand command, CancellationToken ct)
      => PostAsync<BanUserCommand, bool>($"/api/admin/users/{userId}/ban", command, ct);

  public Task<bool> Suspend(Guid userId, SuspendUserCommand command, CancellationToken ct)
      => PostAsync<SuspendUserCommand, bool>($"/api/admin/users/{userId}/suspend", command, ct);

  public Task<bool> Reactivate(Guid userId, CancellationToken ct)
      => PostAsync<ReactivateUserCommand, bool>($"/api/admin/users/{userId}/reactivate", new ReactivateUserCommand(userId), ct);
}