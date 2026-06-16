using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Identity;

public sealed record LinkUserIdentityCommand(
    Guid UserId,
    string Provider,
    string ExternalId,
    string? DeviceId
) : ICommand<Guid>; 