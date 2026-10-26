using Franz.Common.Mapping.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService.Application.Commands.Authentication;
using UserService.Application.Commands.Authentication.Services;
using UserService.Application.Validation;

namespace UserService.Application;

public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
  {
    services.AddFranzMapping(Assembly.GetExecutingAssembly());

    services.AddScoped<IAuthenticationService, AuthenticationService>();
    services.AddScoped<ILoginSessionService, LoginSessionService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IUserAccessValidator, UserAccessValidator>();

    return services;
  }
}