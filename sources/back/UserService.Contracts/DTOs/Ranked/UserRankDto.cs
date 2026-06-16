using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Ranked;

public sealed record UserRankDto
{
  public Guid UserId { get; init; }
  public int MMR { get; init; }
  public int Wins { get; init; }
  public int Losses { get; init; }
}