using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Users;

public sealed record UpdateUsernameCommand(
    Guid UserId,
    string NewUsername
) : ICommand<bool>;