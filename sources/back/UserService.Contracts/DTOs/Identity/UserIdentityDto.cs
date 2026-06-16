using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Identity;

public sealed record UserIdentityDto
{
  public Guid Id { get; init; }
  public Guid UserId { get; init; }
  public string Provider { get; init; } = null!;
  public string ExternalId { get; init; } = null!;
  public string? DeviceId { get; init; }
}