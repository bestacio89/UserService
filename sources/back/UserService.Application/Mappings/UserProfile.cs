using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Users;

using UserService.Domain.Users;

namespace UserService.Application.Mappings.Users;

public sealed class UserProfile : FranzMapProfile
{
  public UserProfile()
  {
    // =========================================================
    // User → DTO
    // =========================================================
    CreateMap<User, UserDto>()
        .ConstructUsing(src => new UserDto
        {
          Id = src.Id,
          Username = src.Username,
          State = src.State.ToString()
        });
  }
}