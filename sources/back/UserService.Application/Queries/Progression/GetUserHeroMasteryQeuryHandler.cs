using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UserService.Application.ReadModels.Progression;
using UserService.Contracts.DTOs.Progression;
using UserService.Contracts.Queries.Progression;
using UserService.Domain.Progression;

namespace UserService.Application.Queries.Progression;

public sealed class GetUserHeroMasteryQueryHandler
    : IQueryHandler<GetUserHeroMasteryQuery, UserHeroMasteryDto?>
{
  private readonly IUserProgressionLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetUserHeroMasteryQueryHandler(
      IUserProgressionLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<UserHeroMasteryDto?> Handle(
      GetUserHeroMasteryQuery query,
      CancellationToken ct)
  {
    var data = await _repository.GetHeroMastery(
        query.UserId,
        query.HeroId,
        ct);

    return data is null
        ? null
        : _mapper.Map<UserHeroMastery, UserHeroMasteryDto>(data);
  }
}
