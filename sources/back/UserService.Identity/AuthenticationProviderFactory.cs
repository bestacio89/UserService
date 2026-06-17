using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Domain.Identity;

namespace UserService.Identity;

public sealed class AuthenticationProviderFactory : IAuthenticationProviderFactory
{
  private readonly Dictionary<IdentityProvider, IAuthenticationProvider> _providers;

  public AuthenticationProviderFactory(IEnumerable<IAuthenticationProvider> providers)
  {
    _providers = providers.ToDictionary(x => x.Provider);
  }

  public IAuthenticationProvider Get(IdentityProvider provider)
  {
    if (!_providers.TryGetValue(provider, out var p))
      throw new InvalidOperationException($"Unknown provider: {provider}");

    return p;
  }
}