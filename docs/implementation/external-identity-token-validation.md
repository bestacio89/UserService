# External identity token validation

Google and Apple login requests must provide an ID token issued for a client application configured for this service. UserService validates the token before reading identity claims.

## Required configuration

Configure client audiences through deployment configuration or environment variables; never infer client IDs from incoming requests.

- `Authentication__Google__ClientIds__0`: an allowed Google OAuth client ID.
- `Authentication__Apple__ClientIds__0`: an allowed Apple Services ID or app client ID.

Add indexed entries for additional audiences. The singular `Authentication__Google__ClientId` and `Authentication__Apple__ClientId` keys are also accepted. Empty arrays in appsettings are placeholders, not usable credentials.

## Validation guarantees

- HTTPS OpenID Connect metadata and provider signing keys.
- Signature, RS256 algorithm, issuer, configured audience, expiry, and not-before validation.
- Separate Google and Apple issuer allowlists and metadata endpoints.
- Fail-closed behavior when audiences are not configured.
- Claims are consumed only from the validated principal; email is not an account merge key.
- No raw provider tokens are logged.

Metadata retrieval requires outbound HTTPS access. The current unit tests cover malformed-token rejection, missing-audience fail-closed behavior, and unsupported providers. Controlled-key integration tests for forged/expired tokens, wrong issuer/audience, and key rotation remain required before security validation is complete.
