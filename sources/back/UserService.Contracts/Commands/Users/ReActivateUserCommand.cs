using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Admin;

public sealed record ReactivateUserCommand(
    Guid UserId
) : ICommand<bool>;