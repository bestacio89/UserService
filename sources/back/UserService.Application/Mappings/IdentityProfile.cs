using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Identity;
using UserService.Domain.Identity;

namespace UserService.Application.Mappings.Identity;

public sealed class IdentityProfile : FranzMapProfile
{
  public IdentityProfile()
  {
    // =========================================================
    // UserIdentity → DTO
    // =========================================================
    CreateMap<UserIdentity, UserIdentityDto>()
        .ConstructUsing(src => new UserIdentityDto
        {
          Id = src.Id,
          UserId = src.UserId,
          Provider = src.Provider.ToString(),
          ExternalId = src.ExternalId,
          DeviceId = src.DeviceId
        });
  }
}