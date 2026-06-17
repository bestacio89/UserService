using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Identity;

namespace UserService.Contracts.DTOs.Authentication;

public sealed record ExternalIdentity(
    IdentityProvider Provider,
    string ProviderUserId,
    string? Email,
    bool EmailVerified
);