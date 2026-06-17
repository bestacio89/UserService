using UserService.Domain.Identity;

namespace UserService.Contracts.Infrastructure.Authentication;

public interface IAuthenticationProviderFactory
{
  IAuthenticationProvider Get(IdentityProvider provider);
}