using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Users;


namespace UserService.Contracts.Queries.Users;

public sealed record GetUserByIdQuery(
    Guid UserId
) : IQuery<UserDto>;