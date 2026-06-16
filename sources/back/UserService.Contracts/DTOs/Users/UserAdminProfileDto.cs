using System;
using System.Collections.Generic;
using System.Text;
using UserService.Contracts.DTOs.Progression;
using UserService.Contracts.DTOs.Ranked;

namespace UserService.Contracts.DTOs.Users;

public sealed record UserAdminProfileDto
{
  public UserDto User { get; init; } = null!;
  public UserRankDto Rank { get; init; } = null!;
  public IReadOnlyList<UserHeroMasteryDto> HeroMasteries { get; init; } = null!;
  public IReadOnlyList<UserClassMasteryDto> ClassMasteries { get; init; } = null!;
  public string AccountState { get; init; } = null!;
}