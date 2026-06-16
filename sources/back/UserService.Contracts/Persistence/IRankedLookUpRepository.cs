using Franz.Common.DependencyInjection;
using UserService.Contracts.DTOs.Ranked;
using UserService.Domain.Ranked;

namespace UserService.Application.ReadModels.Ranked;

public interface IRankedLookupRepository : IScopedDependency
{
  Task<RankedEligibilityResponseDto> CheckEligibility(Guid userId, Guid heroClassId, CancellationToken ct);
  Task<UserRank?> GetRankAsync(Guid userId, CancellationToken ct);
}