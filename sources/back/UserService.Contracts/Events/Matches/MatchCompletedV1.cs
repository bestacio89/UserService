namespace UserService.Contracts.Events.Matches;

/// <summary>
/// Versioned consumer contract for a terminal match result published by MatchService.
/// UserService applies player-owned progression and ranking policies; it does not
/// simulate matches or decide their outcomes.
/// </summary>
public sealed record MatchCompletedV1(
    int SchemaVersion,
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
    public const int CurrentSchemaVersion = 1;
}
