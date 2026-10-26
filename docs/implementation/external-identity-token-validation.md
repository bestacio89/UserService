# External identity token validation

Google and Apple login requests must provide an ID token issued for a client application configured for this service. UserService validates the token before reading the subject, email, or email-verification claim.

## Required configuration

Configure client audiences through deployment configuration or environment variables. Never infer a client ID from the incoming request.

Example environment-variable keys:

- `Authentication__Google__ClientIds__0` — Google OAuth client ID for an allowed client.
- `Authentication__Apple__ClientIds__0` — Apple Services ID or app client ID allowed to authenticate against this service.

Multiple audiences can be configured by adding indexed entries. The singular `Authentication__Google__ClientId` and `Authentication__Apple__ClientId` keys are also accepted for deployments with one audience. The empty arrays in the development appsettings file are deliberate placeholders, not usable credentials.

## Validation guarantees

- HTTPS OpenID Connect metadata and provider signing keys are used; keys are refreshed when an unknown signing key is encountered.
- Signature, allowed RSA-SHA256 algorithm, issuer, audience, expiration, and not-before lifetime are validated.
- Google and Apple use separate metadata endpoints and issuer allowlists.
- An absent audience configuration fails closed.
- Provider subjects and email claims are read only from the validated principal. Email is never used to merge or identify accounts.
- Raw provider tokens are not logged by this implementation.

## Operational notes

Metadata retrieval requires outbound HTTPS access to the provider's OpenID Connect metadata and key endpoints. A metadata/network outage must be monitored and surfaced by the API's normal error handling. Keep provider client IDs in deployment configuration, and do not place production credentials in source control.

The current unit tests cover malformed-token rejection, missing-audience fail-closed behavior, and rejection of non-OIDC providers. Integration tests with controlled signing keys and provider metadata are still required before this change is considered security-validated.
