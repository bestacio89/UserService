using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Progression;


public sealed record AddHeroMasteryXpDto
{
  public Guid UserId { get; init; }
  public Guid HeroId { get; init; }
  public int ExperienceGained { get; init; }
  public bool Won { get; init; }
}