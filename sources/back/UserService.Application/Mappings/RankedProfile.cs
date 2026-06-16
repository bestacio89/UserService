using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Ranked;

using UserService.Domain.Ranked;

namespace UserService.Application.Mappings.Ranked;

public sealed class RankedProfile : FranzMapProfile
{
  public RankedProfile()
  {
    // =========================================================
    // UserRank → DTO
    // =========================================================
    CreateMap<UserRank, UserRankDto>()
        .ConstructUsing(src => new UserRankDto
        {
          UserId = src.UserId,
          MMR = src.MMR,
          Wins = src.Wins,
          Losses = src.Losses
        });
  }
}