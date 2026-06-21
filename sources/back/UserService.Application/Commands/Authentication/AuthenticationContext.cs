using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.Commands.Authentication;
using UserService.Contracts.DTOs.Authentication;
using UserService.Domain.Authentication;
using UserService.Domain.Identity;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication;

public sealed class AuthenticationContext
{
  public LoginUserCommand Command { get; init; }

  public ExternalIdentity ExternalIdentity { get; set; }
  public UserIdentity UserIdentity { get; set; }
  public User User { get; set; }
  public UserSession Session { get; set; }

  public string AccessToken { get; set; }
  public string RefreshToken { get; set; }
}
