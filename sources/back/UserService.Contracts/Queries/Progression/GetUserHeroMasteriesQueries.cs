using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Progression;

namespace UserService.Contracts.Queries.Progression;

public sealed record GetUserHeroMasteriesQuery(
    Guid UserId
) : IQuery<IReadOnlyList<UserHeroMasteryDto>>;