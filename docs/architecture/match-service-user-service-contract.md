# MatchService → UserService integration contract

## Ownership

- **MatchService** owns match lifecycle, authoritative results, selected game version, and the immutable gameplay snapshot used by the match.
- **UserService** owns player progression, hero/class mastery, ranked records, account state, and player-facing match history.
- **HeroService / game configuration services** own hero definitions and game-version/snapshot content.

UserService must not create a second GameVersion or gameplay-snapshot aggregate and must not make match adjudication decisions. It stores references to the authoritative GameVersionId and SnapshotId from the result so a player record can be traced to the exact rules/content used by that match.

## Event contract

MatchCompletedV1 is the versioned consumer contract for an authoritative terminal match result. Its shape is defined under UserService.Contracts.Events.Matches. MatchService should publish the same schema from a neutral shared contracts package or its producer-owned integration contract; it should not depend on UserService's application or domain assemblies to publish an event.

The event contains:

- SchemaVersion and EventId for schema validation and message-level idempotency.
- MatchId for match-level deduplication and history.
- GameVersionId, SnapshotId, and SnapshotHash for auditability without copying the snapshot.
- UTC StartedAt and CompletedAt timestamps.
- GameMode and IsRanked.
- One participant result per player, including selected hero/class, team, authoritative outcome, and gameplay statistics.
- RankedEligible as a MatchService-supplied eligibility fact; UserService still applies its own ranked policy and computes the MMR change.

A future incompatible payload must use a new contract version rather than silently changing the meaning of V1.

## UserService persistence model

UserMatchRecord is a per-user history projection. It retains the match and participant identifiers, final result, relevant statistics, and the game-version/snapshot references needed for support and audit. It deliberately does not embed the snapshot payload or duplicate GameVersion definitions.

The unique database constraint on (MatchId, UserId) prevents a participant result from being recorded twice. Event-level deduplication and applying the history record, progression changes, and ranked changes in one transaction remain requirements for the consumer implementation.

## Processing invariants

1. Validate the schema version, envelope, timestamps, IDs, snapshot hash, and participant uniqueness before applying changes.
2. Reject or quarantine a result with duplicate UserId entries or invalid statistics; do not partially apply it.
3. Deduplicate delivery by EventId and prevent a second application of the same (MatchId, UserId) result.
4. Apply each match result and its progression/ranked updates atomically within UserService's own PostgreSQL transaction.
5. Calculate experience and MMR changes using UserService-owned policies. Do not trust MatchService to dictate awarded XP or MMR deltas.
6. Never write to MatchService's or HeroService's database. Event processing is the cross-service boundary.
7. Do not acknowledge a Kafka message until the application transaction succeeds; failed processing must remain eligible for retry or dead-letter handling.
