using Franz.Common.DependencyInjection;
using UserService.Contracts.DTOs.Users;
using UserService.Domain.Users;

namespace UserService.Contracts.Persistence;

public interface IUserLookupRepository : IScopedDependency
{
  Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);

  Task<User?> GetByUsernameAsync(string username, CancellationToken ct);

  Task<User?> GetAdminProfileAsync(Guid userId, CancellationToken ct);

  Task<UserAccountState?> GetAccountStateAsync(Guid userId, CancellationToken ct);
}