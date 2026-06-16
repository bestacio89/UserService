using Franz.Common.DependencyInjection;
using UserService.Contracts.DTOs.Progression;
using UserService.Domain.Progression;

namespace UserService.Application.ReadModels.Progression;

public interface IUserProgressionLookupRepository : IScopedDependency
{
  Task<IReadOnlyList<UserHeroMastery>> GetHeroMasteries(Guid userId, CancellationToken ct);

  Task<UserHeroMastery> GetHeroMastery(Guid userId, Guid heroId, CancellationToken ct);

  Task<UserClassMastery> GetClassMastery(Guid userId, Guid heroClassId, CancellationToken ct);

  Task<IReadOnlyList<UserClassMastery>> GetClassMasteries(Guid userId, CancellationToken ct);
}