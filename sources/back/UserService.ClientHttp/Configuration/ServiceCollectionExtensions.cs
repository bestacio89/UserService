using Microsoft.Extensions.DependencyInjection;
using UserService.Client.Http.Clients;

namespace UserService.Client.Http.Configuration;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddUserServiceHttpClient(
      this IServiceCollection services,
      string baseUrl)
  {
    services.AddHttpClient<UsersClient>(c => c.BaseAddress = new Uri(baseUrl));
    services.AddHttpClient<AdminUsersClient>(c => c.BaseAddress = new Uri(baseUrl));
    services.AddHttpClient<RankedClient>(c => c.BaseAddress = new Uri(baseUrl));
    services.AddHttpClient<ProgressionClient>(c => c.BaseAddress = new Uri(baseUrl));
    services.AddHttpClient<IdentityClient>(c => c.BaseAddress = new Uri(baseUrl));

    return services;
  }
}