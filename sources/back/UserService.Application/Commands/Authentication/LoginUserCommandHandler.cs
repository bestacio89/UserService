using Franz.Common.Mediator.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.Commands.Authentication.Services;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;

namespace UserService.Application.Commands.Authentication;

public sealed class LoginUserCommandHandler
    : ICommandHandler<LoginUserCommand, LoginResult>
{
  private readonly IAuthenticationService _auth;

  public Task<LoginResult> Handle(LoginUserCommand request, CancellationToken ct)
      => _auth.LoginAsync(request, ct);
}