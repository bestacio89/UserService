using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace UserService.Application.Commands.Authentication.Services;

public interface IAuthenticationStep
{
  Task ExecuteAsync(AuthenticationContext context, CancellationToken ct);
}
