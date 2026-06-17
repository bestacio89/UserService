using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Authentication;

public sealed record LoginResult(
    Guid UserId,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
