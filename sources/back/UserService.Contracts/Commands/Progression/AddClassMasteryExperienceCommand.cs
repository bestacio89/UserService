using Franz.Common.Mediator.Messages;

namespace UserService.Contracts.Commands.Progression;

public sealed record AddClassMasteryExperienceCommand(
    Guid UserId,
    Guid HeroClassId,
    int ExperienceGained
) : ICommand<bool>; 