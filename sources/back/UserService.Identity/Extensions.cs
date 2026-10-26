using Microsoft.Extensions.DependencyInjection;
using UserService.Contracts.Infrastructure.Authentication;
using UserService.Identity.Apple;
using UserService.Identity.Google;

namespace UserService.Identity;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddIdentityProviders(this IServiceCollection services)
  {
    services.AddSingleton<ExternalIdentityTokenValidator>();
    services.AddScoped<IAuthenticationProvider, AppleAuthenticationProvider>();
    services.AddScoped<IAuthenticationProvider, GoogleAuthenticationProvider>();
    services.AddScoped<IAuthenticationProviderFactory, AuthenticationProviderFactory>();

    return services;
  }
}
