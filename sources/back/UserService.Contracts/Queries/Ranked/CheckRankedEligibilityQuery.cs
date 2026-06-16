using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Ranked;


namespace UserService.Contracts.Queries.Ranked;

public sealed record CheckRankedEligibilityQuery(
    Guid UserId,
    Guid HeroClassId
) : IQuery<RankedEligibilityResponseDto>;