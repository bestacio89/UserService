namespace UserService.Contracts.Events.Matches;

/// <summary>
/// Terminal outcome assigned by MatchService. This is a result fact, not a ranking
/// or progression policy decision.
/// </summary>
public enum MatchParticipantOutcome
{
    Victory = 1,
    Defeat = 2,
    Draw = 3,
    Abandoned = 4,
    Disconnected = 5
}
