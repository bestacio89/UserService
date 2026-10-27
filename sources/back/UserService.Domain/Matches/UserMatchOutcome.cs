namespace UserService.Domain.Matches;

/// <summary>
/// A player's terminal outcome as recorded by UserService from MatchService's result.
/// Kept in the domain layer to avoid coupling domain entities to transport contracts.
/// </summary>
public enum UserMatchOutcome
{
    Victory = 1,
    Defeat = 2,
    Draw = 3,
    Abandoned = 4,
    Disconnected = 5
}
