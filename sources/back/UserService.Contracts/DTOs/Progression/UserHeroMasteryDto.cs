using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Progression;

public sealed record UserHeroMasteryDto
{
  public Guid UserId { get; init; }
  public Guid HeroId { get; init; }
  public int MasteryLevel { get; init; }
  public int Experience { get; init; }
  public int GamesPlayed { get; init; }
}
