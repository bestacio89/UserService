using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Progression;
using UserService.Application.ReadModels.Ranked;
using UserService.Application.ReadModels.Users;
using UserService.Contracts.DTOs.Progression;
using UserService.Contracts.DTOs.Ranked;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Admin;
using UserService.Domain.Progression;
using UserService.Domain.Ranked;
using UserService.Domain.Users;

namespace UserService.Application.Queries.Admin;

public sealed class GetUserAdminProfileQueryHandler
    : IQueryHandler<GetUserAdminProfileQuery, UserAdminProfileDto>
{
  private readonly IUserLookupRepository _userRepo;
  private readonly IUserProgressionLookupRepository _progressionRepo;
  private readonly IRankedLookupRepository _rankRepo;
  private readonly IFranzMapper _mapper;

  public GetUserAdminProfileQueryHandler(
      IUserLookupRepository userRepo,
      IUserProgressionLookupRepository progressionRepo,
      IRankedLookupRepository rankRepo,
      IFranzMapper mapper)
  {
    _userRepo = userRepo;
    _progressionRepo = progressionRepo;
    _rankRepo = rankRepo;
    _mapper = mapper;
  }

  public async Task<UserAdminProfileDto> Handle(
      GetUserAdminProfileQuery query,
      CancellationToken ct)
  {
    var user = await _userRepo.GetByIdAsync(query.UserId, ct);
    if (user is null) throw new Exception("User not found");

    var rank = await _rankRepo.GetRankAsync(query.UserId, ct);

    var heroMasteries = await _progressionRepo.GetHeroMasteries(query.UserId, ct);
    var classMasteries = await _progressionRepo.GetClassMasteries(query.UserId, ct);

    return new UserAdminProfileDto
    {
      User = _mapper.Map<User, UserDto>(user),
      Rank = rank is null
            ? new UserRankDto { UserId = query.UserId }
            : _mapper.Map<UserRank, UserRankDto>(rank),

      HeroMasteries = heroMasteries
            .Select(x => _mapper.Map<UserHeroMastery, UserHeroMasteryDto>(x))
            .ToList(),

      ClassMasteries = classMasteries
            .Select(x => _mapper.Map<UserClassMastery, UserClassMasteryDto>(x))
            .ToList(),

      AccountState = user.State.ToString()
    };
  }
}