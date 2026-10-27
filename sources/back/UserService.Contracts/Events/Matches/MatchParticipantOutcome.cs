namespace UserService.Contracts.Events.Matches;

/// <summary>
/// Terminal outcome assigned by MatchService, independent of UserService ranking policy.
/// </summary>
public enum MatchParticipantOutcome
{
    Victory = 1,
    Defeat = 2,
    Draw = 3,
    Abandoned = 4,
    Disconnected = 5
}
