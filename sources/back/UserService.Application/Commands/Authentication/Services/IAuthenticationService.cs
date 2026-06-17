using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;

namespace UserService.Application.Commands.Authentication.Services;

public interface IAuthenticationService
{
  Task<LoginResult> LoginAsync(LoginUserCommand command, CancellationToken ct);
}