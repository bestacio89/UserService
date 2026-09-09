using Franz.Common.DependencyInjection;
using UserService.Domain.Wallet;

namespace UserService.Contracts.Persistence;

public interface ICurrencyLookupRepository : IScopedDependency
{
  Task<Currency?> GetByCodeAsync(string code, CancellationToken ct);

  Task<IReadOnlyList<Currency>> GetActiveAsync(CancellationToken ct);
}