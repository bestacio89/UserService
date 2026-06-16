using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Admin;

public sealed record BanUserCommand(
    Guid UserId,
    string Reason
) : ICommand<bool>;