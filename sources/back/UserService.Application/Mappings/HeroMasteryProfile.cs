using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Progression;
using UserService.Domain.Progression;

namespace UserService.Application.Mappings.Progression;

public sealed class HeroMasteryProfile : FranzMapProfile
{
  public HeroMasteryProfile()
  {
    // =========================================================
    // UserHeroMastery → DTO
    // =========================================================
    CreateMap<UserHeroMastery, UserHeroMasteryDto>()
        .ConstructUsing(src => new UserHeroMasteryDto
        {
          UserId = src.UserId,
          HeroId = src.HeroId,
          MasteryLevel = src.MasteryLevel,
          Experience = src.Experience,
          GamesPlayed = src.GamesPlayed
        });
  }
}