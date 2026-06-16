using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Progression;

namespace UserService.Contracts.Queries.Progression;

public sealed record GetUserHeroMasteryQuery(
    Guid UserId,
    Guid HeroId
) : IQuery<UserHeroMasteryDto>;