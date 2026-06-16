using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Contracts.DTOs.Ranked;

public sealed record ApplyMatchResultDto
{
  public Guid UserId { get; init; }
  public bool Won { get; init; }
  public int MmrDelta { get; init; }
}