using Franz.Common.DependencyInjection;
using UserService.Contracts.DTOs.Users;
using UserService.Domain.Users;

namespace UserService.Application.ReadModels.Users;

public interface IUserLookupRepository : IScopedDependency
{
  Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);

  Task<User?> GetByUsernameAsync(string username, CancellationToken ct);

  Task<User?> GetAdminProfileAsync(Guid userId, CancellationToken ct);

  Task<UserAccountState?> GetAccountStateAsync(Guid userId, CancellationToken ct);
}