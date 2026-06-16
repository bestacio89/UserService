using Franz.Common.Mediator.Messages;
using UserService.Domain.Users;

namespace UserService.Contracts.Queries.Admin;

public sealed record GetUserAccountStateQuery(
    Guid UserId
) : IQuery<UserAccountState?>;