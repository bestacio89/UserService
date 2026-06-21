using Franz.Common.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Identity;

namespace UserService.Contracts.Persistence;

public interface IUserIdentityLookupRepository : IScopedDependency
{
  Task<UserIdentity?> GetByProviderAsync(
       IdentityProvider provider,
       string providerUserId,
       CancellationToken ct);
}