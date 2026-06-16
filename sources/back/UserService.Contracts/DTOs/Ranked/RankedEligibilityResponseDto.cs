using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Ranked;

public sealed record RankedEligibilityResponseDto
{
  public bool CanQueueRanked { get; init; }
  public string? Reason { get; init; }
}