using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Identity;

public sealed record CreateGuestIdentityCommand(
    string DeviceId
) : ICommand<Guid>;