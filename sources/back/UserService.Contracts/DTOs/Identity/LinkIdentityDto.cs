using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Identity;

public sealed record LinkIdentityDto
{
  public Guid UserId { get; init; }
  public string Provider { get; init; } = null!;
  public string ExternalId { get; init; } = null!;
  public string? DeviceId { get; init; }
}