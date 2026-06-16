using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Progression;


namespace UserService.Contracts.Queries.Progression;

public sealed record GetUserClassMasteryQuery(
    Guid UserId,
    Guid HeroClassId
) : IQuery<UserClassMasteryDto>;