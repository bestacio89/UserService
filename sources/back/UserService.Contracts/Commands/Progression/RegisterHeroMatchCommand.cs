using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Progression;

public sealed record RegisterHeroMatchCommand(
    Guid UserId,
    Guid HeroId,
    bool Won,
    int ExperienceGained
) : ICommand<bool>; 