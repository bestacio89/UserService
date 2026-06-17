using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Authentication;

public sealed record LoginResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);