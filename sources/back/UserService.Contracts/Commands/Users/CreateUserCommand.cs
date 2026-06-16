using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Users;

public sealed record CreateUserCommand(
    string Username
) : ICommand<Guid>;