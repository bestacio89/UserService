namespace UserService.Contracts.Events.Matches;

/// <summary>
/// The authoritative per-player result and gameplay summary supplied by MatchService.
/// UserService computes progression awards and MMR changes from these result facts.
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
