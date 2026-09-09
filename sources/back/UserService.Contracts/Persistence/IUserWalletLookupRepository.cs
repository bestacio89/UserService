using Franz.Common.DependencyInjection;
using UserService.Domain.Wallet;

namespace UserService.Contracts.Persistence;

public interface IUserWalletLookupRepository : IScopedDependency
{
  Task<IReadOnlyList<UserWalletBalance>> GetBalancesAsync(Guid userId, CancellationToken ct);

  Task<UserWalletBalance?> GetBalanceAsync(Guid userId, Guid currencyId, CancellationToken ct);
}