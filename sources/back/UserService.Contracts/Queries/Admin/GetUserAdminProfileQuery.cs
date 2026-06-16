using Franz.Common.Mediator.Messages;

using UserService.Contracts.DTOs.Users;

namespace UserService.Contracts.Queries.Admin;

public sealed record GetUserAdminProfileQuery(
    Guid UserId
) : IQuery<UserAdminProfileDto>;