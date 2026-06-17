using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.DTOs.Authentication;
using UserService.Domain.Identity;

namespace UserService.Contracts.Commands.Authentication;

public sealed record LoginUserCommand(
    IdentityProvider Provider,
    string Identifier,
    string Secret,
    string? DeviceId
) : ICommand<LoginResult>;