# UserService Identity, Entitlements, and Account Consolidation Roadmap

## Goal

Evolve UserService into the authoritative player-account boundary for the MOBA: canonical player accounts, secure authentication, linked provider identities, ownership records, and player entitlements. Preserve the existing .NET 10, Franz.Common, layered architecture, PostgreSQL persistence, and current service boundaries.

This document is a delivery plan. It does not claim implementation or test completion.

## Architecture invariants

1. **Identity is not the account.** A canonical User ID remains stable when a player links Google or Apple, upgrades a guest account, or completes a controlled account merge.
2. **Provider subject is the identity key.** Never silently merge by email. Email is optional profile/contact data and can be absent, changed, or relay-based.
3. **Verify before trusting.** Parse-only JWT handling is not authentication. Validate signature, issuer, audience/client ID, expiry/not-before, and provider-specific claims using trusted, refreshable signing keys.
4. **Linking and merging are different use cases.** Linking attaches a newly verified provider identity to the currently authenticated account. Merging reconciles two pre-existing player accounts through an explicit, auditable workflow.
5. **Ownership is not a purchase.** EconomyService remains authoritative for payment/purchase outcomes and currency transactions. UserService records trusted ownership/entitlement grants and their provenance; it must not invent successful purchases.
6. **Definitions and runtime decisions remain elsewhere.** HeroService/ItemService own asset definitions and gameplay constraints. MatchService remains authoritative for runtime simulation and match decisions.
7. **No cross-service database writes.** Cross-service coordination uses versioned contracts/events and idempotent consumers, not direct writes into another service's persistence.
8. **Franz.Common conventions are mandatory.** Keep existing DI lifetime markers, mediator/mapper patterns, repository abstractions, entity conventions, and architecture tests. Do not add a competing mediator, mapper, or persistence framework.
9. **PostgreSQL is the integration-test reference provider.** Use real PostgreSQL integration tests (Testcontainers where the existing CI supports it), not SQLite substitutions for persistence behavior.
10. **Fail closed.** Suspended/banned account state, revoked sessions, untrusted provider claims, and ambiguous merge outcomes must not grant access.

## Branch and PR sequence

Merge each PR into `develop` before creating the next branch from the updated `develop`. This keeps the history linear and avoids long-lived stacked PRs. Each PR must be independently buildable and have a focused diff.

### PR 0 — This roadmap

- **Branch:** `docs/user-service-identity-entitlement-roadmap`
- **Title:** `[Docs][Domain]: define identity and entitlement roadmap`
- **Scope:** document the service boundaries, invariants, ordered implementation branches, dependency policy, and acceptance gates.
- **Validation:** review the plan against the existing architecture document and project structure.

### PR 1 — Dependency baseline and Franz.Common 2.3.1

- **Branch:** `chore/dependency-baseline-franz-2-3-1`
- **Title:** `[Chore][Domain]: align dependencies with current stable versions`
- **Scope:**
  - Upgrade every referenced `Franz.Common.*` package to **2.3.1**, including explicit package entries and wildcard/fallback versions in `Directory.Packages.props`. Ensure no stale 2.2.x pin remains for a package used by this solution.
  - Audit every direct and transitive NuGet dependency across all solution projects, test projects, tools, and CI. Update to the latest stable versions compatible with .NET 10 and each package's support policy; do not upgrade to prerelease versions or mix incompatible major versions.
  - Inspect central package management, project-level package references, SDK pinning, lock files (if used), Docker/build images, and generated-client tooling for hidden version drift.
  - Use `dotnet list package --outdated` and `dotnet list package --vulnerable` (or the equivalent supported by the pinned SDK), restore, build, and run all tests. Record any dependency intentionally held back with its compatibility reason.
  - Confirm the published availability and compatibility of each Franz.Common 2.3.1 package actually referenced before merging; do not silently substitute a different version.
- **Acceptance:** central package versions are consistent; restore has no downgrade/conflict warnings; no known vulnerable package is left without a documented, accepted exception; full solution build and tests pass.
- **Note:** the current central file contains mixed Franz pins (wildcard 2.2.6 and many explicit 2.2.15 entries), and several older ASP.NET/IdentityModel package pins. The exact target versions for non-Franz packages must be verified against the NuGet feed during implementation, not guessed in this plan.

### PR 2 — Verified Google and Apple authentication

- **Branch:** `fix/external-identity-token-validation`
- **Title:** `[Fix][Application]: validate external identity tokens`
- **Scope:** replace parse-only JWT reads in `UserService.Identity` with provider-specific validation using maintained validation libraries and trusted OpenID Connect metadata/JWKS. Validate Google and Apple issuer, signature, audience/client configuration, lifetime, and required subject; enforce supported algorithms and sensible clock skew. Treat provider errors as safe authentication failures and honor cancellation.
- **Contracts/configuration:** distinguish client IDs/audiences from client secrets; support environment-based configuration; never log raw ID tokens, refresh tokens, or secrets. Avoid treating the API's `identifier` input as trusted identity evidence.
- **Tests:** valid tokens; forged signature; wrong issuer/audience; expired/not-yet-valid token; missing subject; key rotation; unavailable metadata; Apple relay email; Google email verification semantics.
- **Acceptance:** no unvalidated token can create a session or resolve a player identity; no network/JWKS failure silently degrades validation.

### PR 3 — Session and account-state enforcement

- **Branch:** `fix/session-lifecycle-and-account-state`
- **Title:** `[Fix][Application]: enforce session lifecycle and account state`
- **Scope:** review login orchestration, access-token claims, refresh-token storage, rotation/reuse detection, revocation, logout, device/session handling, expiration, and authentication failure behavior. Ensure banned/suspended users cannot obtain or refresh usable sessions. Replace unsafe enum parsing with validated provider binding and consistent 400/401/403 behavior.
- **Security:** store refresh tokens as hashes or equivalent non-recoverable verifiers; make rotation/revocation atomic and concurrency-safe; do not put provider tokens into the game's own access token.
- **Tests:** refresh rotation and replay; revoked session; concurrent refresh; banned/suspended account; malformed provider; cancellation and failure paths.
- **Acceptance:** session state is durable, revocable, and tested against concurrent requests.

### PR 4 — Guest account upgrade and secure identity linking

- **Branch:** `feat/secure-identity-linking`
- **Title:** `[Feat][Application]: link verified provider identities safely`
- **Scope:** introduce authenticated account-scoped link/unlink commands and endpoints. Derive the target User ID from the authenticated principal, never from an untrusted request body. Verify control of the provider identity as part of the linking flow. Enforce one owner per (provider, provider subject), database uniqueness, transactional writes, idempotency, and clear conflict responses.
- **Guest upgrade:** attach a verified Google/Apple identity to the existing guest User ID without resetting progression, wallet references, inventory, or entitlements.
- **Unlink policy:** prevent removal of the last usable sign-in method; require recent/re-authenticated proof as appropriate; audit security-sensitive changes.
- **Tests:** link Google then Apple; already-linked identity; identity owned by another account; forged User ID; duplicate concurrent link; guest upgrade preserves canonical User ID; unlink last method denied.
- **Acceptance:** no client can attach an identity to an arbitrary player or take over an existing identity.

### PR 5 — Entitlement and ownership domain

- **Branch:** `feat/player-entitlements-and-ownership`
- **Title:** `[Feat][Domain]: model player ownership and entitlements`
- **Scope:** define separate domain concepts for entitlement definition/reference, player grant, grant source/provenance, validity, revocation, and ownership queries. Keep asset definitions in HeroService/ItemService and payment/order authority in EconomyService.
- **Domain rules:** represent permanent and time-bound grants, source references (purchase, gameplay reward, event, administrator/compensation), effective/revoked/expired states, and audit metadata. Avoid a single boolean per entitlement or duplicating the asset catalog. Define stable identifiers and uniqueness/idempotency rules for external grant references.
- **API/contracts:** add focused grant/revoke/query contracts and a clear effective-access query. Mutating operations must be authenticated/authorized service-to-service operations, not arbitrary client grant endpoints.
- **Persistence:** PostgreSQL schema/migrations, required unique constraints and indexes, transaction boundaries, concurrency handling, and query patterns. Use the repository's existing Franz.Common persistence and entity conventions.
- **Tests:** duplicate grant; expiry boundary; revocation; concurrent duplicate delivery; unknown asset reference; invalid source; account-state restrictions; audit provenance.
- **Acceptance:** every entitlement is explainable by a source, can be queried consistently, and cannot be granted by an untrusted client.

### PR 6 — Trusted economy integration and ownership projections

- **Branch:** `feat/economy-entitlement-integration`
- **Title:** `[Feat][Messaging]: consume trusted entitlement grant events`
- **Scope:** define versioned contracts with EconomyService for verified purchase/refund/revocation outcomes and with reward-producing services for eligible grants. Use existing Franz.Common messaging/outbox conventions where present. Implement idempotent event consumption and durable processing state.
- **Rules:** payment processing, pricing, currency ledger, refunds, and fraud checks stay in EconomyService. UserService applies ownership/entitlement projections only from trusted, authenticated events or service commands. Do not grant on a client-supplied receipt or an untrusted event payload alone.
- **Failure handling:** duplicate/out-of-order delivery, retries, poison messages, event versioning, observability, reconciliation, and compensation/revocation policy.
- **Acceptance:** replaying an event does not duplicate ownership; refunds/revocations follow explicit policy; no cross-service database coupling.

### PR 7 — Account merge workflow and reconciliation

- **Branch:** `feat/player-account-merge-workflow`
- **Title:** `[Feat][Application]: orchestrate auditable player account merges`
- **Scope:** add an explicit merge request and durable state machine. Require proof of control of both accounts (or a carefully specified recovery process), re-authentication, idempotency key, designated survivor, policy version, and audit trail. Linking must not automatically trigger a merge.
- **Workflow states:** requested, validating, in progress, completed, failed/retryable, and requires manual resolution. Define legal transitions and prevent concurrent merges involving the same account.
- **Reconciliation:** specify domain policies before moving data. Preserve provenance and prevent duplicate grants. Currency is not blindly summed; paid purchases, refunds, non-transferable grants, ranked data, progression, mastery, inventory, and account sanctions need explicit per-domain rules and source-service coordination. The workflow must not mutate other services' databases.
- **Cross-service coordination:** use durable events/commands and acknowledgements; define idempotent participants, retry/reconciliation operations, and recovery from partial completion. Keep source-account tombstone/redirect semantics and audit history so old IDs cannot be reused for authentication.
- **Tests:** self-merge; insufficient proof; merge conflict; duplicate request; simultaneous merges; partial participant failure/retry; duplicate inventory/grants; suspended/banned source or destination; recovery and audit history.
- **Acceptance:** a merge is recoverable, auditable, deterministic under retries, and cannot silently destroy or duplicate value.

### PR 8 — End-to-end verification, threat model, and operational readiness

- **Branch:** `test/user-service-identity-entitlement-hardening`
- **Title:** `[Test][Tests]: verify identity and entitlement invariants`
- **Scope:** complete PostgreSQL-backed integration tests, API contract tests, architecture tests, threat model, structured security logging, metrics, and operational documentation. Add tests for provider outage/key rotation, database uniqueness races, message replay, account takeover attempts, and merge recovery.
- **CI:** restore/build/test the solution; enforce formatting/analyzers already adopted by the repository; run migration validation and security/dependency checks. Keep tests deterministic and avoid live Google/Apple credentials in CI.
- **Acceptance:** all previous PR acceptance criteria pass in CI; no secrets are committed; runbooks cover key/provider outage, token/session revocation, entitlement reconciliation, and merge recovery.

## Dependency and version policy

The dependency PR is a prerequisite for feature work.

- Keep the solution on **.NET 10**; do not move to .NET 11 previews as part of this scope.
- Set every consumed Franz.Common package to **2.3.1**, including explicit and wildcard/fallback entries, after verifying that each referenced package version is published and compatible.
- Update all other libraries to the latest **stable, compatible** release available at implementation time. Prefer coherent Microsoft package families and compatible test-tool versions; do not blindly force identical versions across unrelated package families.
- Audit transitive dependencies and security advisories, not only `Directory.Packages.props`.
- Regenerate/validate lock files if the repository uses them. Keep reproducible CI restores and document any intentional pin.
- Run `dotnet restore`, `dotnet build UserService.sln`, all test projects, `dotnet list package --outdated`, and `dotnet list package --vulnerable` (adapt command syntax to the selected .NET 10 SDK).
- Check generated NSwag clients and Docker/build images after dependency changes.

## Global Definition of Done

- Each PR has a narrow scope, an updated description, tests, and no unrelated cleanup.
- PRs target `develop`; create each next branch only after the preceding PR is merged.
- Keep commit headers in the repository's agreed format: `[Action][Scope]: lowercase summary` (maximum 50 characters), with body bullets explaining technical what and architectural why.
- Do not bypass existing Franz.Common DI lifetime markers, architecture tests, repository conventions, or PostgreSQL behavior.
- Do not claim production readiness based on compilation alone; token validation, data invariants, concurrency, recovery, and integration behavior require tests.
- No PR may merge with an unresolved security-critical failure or an undocumented compatibility exception.
