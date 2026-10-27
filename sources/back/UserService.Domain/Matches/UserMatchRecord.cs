using Franz.Common.Business.Domain;

namespace UserService.Domain.Matches;

/// <summary>
/// Player-owned historical projection of one participant's authoritative match result.
/// Game version and snapshot content remain owned by MatchService; this record stores
/// only their immutable identifiers and snapshot hash for traceability.
/// </summary>
public sealed class UserMatchRecord : Entity<Guid>
{
    public Guid EventId { get; private set; }
    public Guid MatchId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameVersionId { get; private set; }
    public Guid SnapshotId { get; private set; }
    public string SnapshotHash { get; private set; } = null!;
    public string GameMode { get; private set; } = null!;
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset CompletedAt { get; private set; }
    public bool IsRanked { get; private set; }
    public bool RankedEligible { get; private set; }
    public Guid HeroId { get; private set; }
    public Guid HeroClassId { get; private set; }
    public int TeamId { get; private set; }
    public UserMatchOutcome Outcome { get; private set; }
    public int Kills { get; private set; }
    public int Deaths { get; private set; }
    public int Assists { get; private set; }
    public int ObjectiveScore { get; private set; }

    private UserMatchRecord() { }

    public void Define(
        Guid eventId,
        Guid matchId,
        Guid userId,
        Guid gameVersionId,
        Guid snapshotId,
        string snapshotHash,
        string gameMode,
        DateTimeOffset startedAt,
        DateTimeOffset completedAt,
        bool isRanked,
        bool rankedEligible,
        Guid heroId,
        Guid heroClassId,
        int teamId,
        UserMatchOutcome outcome,
        int kills,
        int deaths,
        int assists,
        int objectiveScore)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("A valid event ID is required.", nameof(eventId));
        if (matchId == Guid.Empty) throw new ArgumentException("A valid match ID is required.", nameof(matchId));
        if (userId == Guid.Empty) throw new ArgumentException("A valid user ID is required.", nameof(userId));
        if (gameVersionId == Guid.Empty) throw new ArgumentException("A valid game version ID is required.", nameof(gameVersionId));
        if (snapshotId == Guid.Empty) throw new ArgumentException("A valid snapshot ID is required.", nameof(snapshotId));
        if (heroId == Guid.Empty) throw new ArgumentException("A valid hero ID is required.", nameof(heroId));
        if (heroClassId == Guid.Empty) throw new ArgumentException("A valid hero class ID is required.", nameof(heroClassId));
        if (string.IsNullOrWhiteSpace(snapshotHash)) throw new ArgumentException("A snapshot hash is required.", nameof(snapshotHash));
        if (string.IsNullOrWhiteSpace(gameMode)) throw new ArgumentException("A game mode is required.", nameof(gameMode));
        if (startedAt == default || completedAt < startedAt) throw new ArgumentOutOfRangeException(nameof(completedAt), "The completion timestamp must not precede the start timestamp.");
        if (startedAt.Offset != TimeSpan.Zero || completedAt.Offset != TimeSpan.Zero) throw new ArgumentException("Match timestamps must be UTC.");
        if (teamId < 0) throw new ArgumentOutOfRangeException(nameof(teamId));
        if (kills < 0) throw new ArgumentOutOfRangeException(nameof(kills));
        if (deaths < 0) throw new ArgumentOutOfRangeException(nameof(deaths));
        if (assists < 0) throw new ArgumentOutOfRangeException(nameof(assists));
        if (objectiveScore < 0) throw new ArgumentOutOfRangeException(nameof(objectiveScore));
        if (!Enum.IsDefined(outcome)) throw new ArgumentOutOfRangeException(nameof(outcome));
        if (!isRanked && rankedEligible) throw new ArgumentException("A participant cannot be ranked-eligible in an unranked match.", nameof(rankedEligible));

        EventId = eventId;
        MatchId = matchId;
        UserId = userId;
        GameVersionId = gameVersionId;
        SnapshotId = snapshotId;
        SnapshotHash = snapshotHash.Trim();
        GameMode = gameMode.Trim();
        StartedAt = startedAt;
        CompletedAt = completedAt;
        IsRanked = isRanked;
        RankedEligible = rankedEligible;
        HeroId = heroId;
        HeroClassId = heroClassId;
        TeamId = teamId;
        Outcome = outcome;
        Kills = kills;
        Deaths = deaths;
        Assists = assists;
        ObjectiveScore = objectiveScore;

        MarkCreated("system");
    }
}
