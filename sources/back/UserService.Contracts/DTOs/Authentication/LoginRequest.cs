using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Authentication;

public sealed record LoginRequest(
    string Provider,
    string Identifier,
    string Secret,
    string? DeviceId
);