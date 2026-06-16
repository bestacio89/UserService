using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Progression;
using UserService.Domain.Progression;

namespace UserService.Application.Mappings.Progression;

public sealed class ClassMasteryProfile : FranzMapProfile
{
  public ClassMasteryProfile()
  {
    // =========================================================
    // UserClassMastery → DTO
    // =========================================================
    CreateMap<UserClassMastery, UserClassMasteryDto>()
        .ConstructUsing(src => new UserClassMasteryDto
        {
          UserId = src.UserId,
          HeroClassId = src.HeroClassId,
          MasteryLevel = src.MasteryLevel,
          Experience = src.Experience
        });
  }
}