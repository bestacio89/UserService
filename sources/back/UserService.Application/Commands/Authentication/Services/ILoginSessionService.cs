using System;
using System.Collections.Generic;
using System.Text;
using UserService.Domain.Authentication;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Authentication.Services;

public interface ILoginSessionService
{
  UserSession CreateSession(User user, string? deviceId);

  Task PersistAsync(UserSession session, CancellationToken ct);
}