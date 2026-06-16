using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Admin;

public sealed record SuspendUserCommand(
    Guid UserId,
    string Reason,
    DateTime? Until
) : ICommand<bool>; 