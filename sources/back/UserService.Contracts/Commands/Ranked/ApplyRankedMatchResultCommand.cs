using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Ranked;


namespace UserService.Contracts.Queries.Ranked;

public sealed record GetUserRankQuery(
    Guid UserId
) : IQuery<UserRankDto>;