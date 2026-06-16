using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Ranked;


public sealed record RankedEligibilityRequestDto
{
  public Guid UserId { get; init; }
  public Guid HeroClassId { get; init; }
}