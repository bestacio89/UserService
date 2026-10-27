namespace UserService.Contracts.Events.Matches;

/// <summary>
/// Versioned integration contract emitted by MatchService after a match has reached
/// an authoritative terminal state. UserService consumes this contract to update
/// player-owned progression and ranked records; it does not simulate or adjudicate matches.
/// </summary>
public sealed record MatchCompletedV1(
    Guid EventId,
    Guid MatchId,
    Guid GameVersionId,
    Guid SnapshotId,
    string SnapshotHash,
    DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt,
    string GameMode,
    bool IsRanked,
    IReadOnlyList<MatchParticipantResultV1> Participants)
{
    public const int SchemaVersion = 1;
}
