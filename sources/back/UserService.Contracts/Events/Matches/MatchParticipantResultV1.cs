namespace UserService.Contracts.Events.Matches;

/// <summary>
/// The authoritative, per-player outcome and gameplay summary supplied by MatchService.
/// Progression awards and MMR changes remain UserService decisions.
/// </summary>
public sealed record MatchParticipantResultV1(
    Guid UserId,
    Guid HeroId,
    Guid HeroClassId,
    int TeamId,
    MatchParticipantOutcome Outcome,
    int Kills,
    int Deaths,
    int Assists,
    int ObjectiveScore,
    bool RankedEligible);
