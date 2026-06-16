using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Progression;

public sealed record UserClassMasteryDto
{
  public Guid UserId { get; init; }
  public Guid HeroClassId { get; init; }
  public int MasteryLevel { get; init; }
  public int Experience { get; init; }
}