using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.DTOs.Authentication;
using UserService.Domain.Identity;

namespace UserService.Contracts.Infrastructure.Authentication;

public interface IAuthenticationProvider
{
  IdentityProvider Provider { get; }

  Task<ExternalIdentity> AuthenticateAsync(
      string identifier,
      string secret,
      CancellationToken ct);
}