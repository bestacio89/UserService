using UserService.Domain.Matches;
using Xunit;

namespace UserService.Testing.DomainTests;

public sealed class UserMatchRecordTests
{
    [Fact]
    public void Define_ShouldPreserveMatchAndSnapshotProvenance()
    {
        var record = (UserMatchRecord)Activator.CreateInstance(typeof(UserMatchRecord), nonPublic: true)!;
        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-20);
        var completedAt = startedAt.AddMinutes(18);

        record.Define(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "sha256:match-snapshot",
            "ranked",
            startedAt,
            completedAt,
            true,
            true,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            UserMatchOutcome.Victory,
            8,
            2,
            11,
            4);

        Assert.NotEqual(Guid.Empty, record.MatchId);
        Assert.NotEqual(Guid.Empty, record.UserId);
        Assert.NotEqual(Guid.Empty, record.GameVersionId);
        Assert.NotEqual(Guid.Empty, record.SnapshotId);
        Assert.Equal("sha256:match-snapshot", record.SnapshotHash);
        Assert.Equal(completedAt, record.CompletedAt);
        Assert.Equal(UserMatchOutcome.Victory, record.Outcome);
        Assert.Equal(8, record.Kills);
        Assert.Equal(2, record.Deaths);
        Assert.Equal(11, record.Assists);
    }

    [Fact]
    public void Define_ShouldRejectCompletionBeforeStart()
    {
        var record = new UserMatchRecord();
        var startedAt = DateTimeOffset.UtcNow;

        Assert.Throws<ArgumentOutOfRangeException>(() => record.Define(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "sha256:match-snapshot",
            "ranked",
            startedAt,
            startedAt.AddSeconds(-1),
            true,
            true,
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            UserMatchOutcome.Defeat,
            0,
            0,
            0,
            0));
    }

    [Fact]
    public void Define_ShouldRejectNegativeGameplayStatistics()
    {
        var record = new UserMatchRecord();
        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-10);

        Assert.Throws<ArgumentOutOfRangeException>(() => record.Define(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "sha256:match-snapshot",
            "ranked",
            startedAt,
            startedAt.AddMinutes(10),
            true,
            true,
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            UserMatchOutcome.Victory,
            -1,
            0,
            0,
            0));
    }

    [Fact]
    public void Define_ShouldRejectRankedEligibilityForUnrankedMatch()
    {
        var record = new UserMatchRecord();
        var startedAt = DateTimeOffset.UtcNow.AddMinutes(-10);

        Assert.Throws<ArgumentException>(() => record.Define(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "sha256:match-snapshot",
            "casual",
            startedAt,
            startedAt.AddMinutes(10),
            false,
            true,
            Guid.NewGuid(),
            Guid.NewGuid(),
            0,
            UserMatchOutcome.Victory,
            1,
            0,
            0,
            0));
    }
}
