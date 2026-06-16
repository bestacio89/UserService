
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Progression;
using UserService.Contracts.DTOs.Progression;
using UserService.Contracts.Queries.Progression;
using UserService.Domain.Progression;


namespace UserService.Application.Queries.Progression;

public sealed class GetUserHeroMasteriesQueryHandler
    : IQueryHandler<GetUserHeroMasteriesQuery, IReadOnlyList<UserHeroMasteryDto>>
{
  private readonly IUserProgressionLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetUserHeroMasteriesQueryHandler(
      IUserProgressionLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<UserHeroMasteryDto>> Handle(
      GetUserHeroMasteriesQuery query,
      CancellationToken ct)
  {
    var data = await _repository.GetHeroMasteries(query.UserId, ct);

    return data
        .Select(x => _mapper.Map<UserHeroMastery, UserHeroMasteryDto>(x))
        .ToList();
  }
}