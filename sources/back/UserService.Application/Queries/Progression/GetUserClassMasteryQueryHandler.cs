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

public sealed class GetUserClassMasteryQueryHandler
    : IQueryHandler<GetUserClassMasteryQuery, UserClassMasteryDto?>
{
  private readonly IUserProgressionLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetUserClassMasteryQueryHandler(
      IUserProgressionLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<UserClassMasteryDto?> Handle(
      GetUserClassMasteryQuery query,
      CancellationToken ct)
  {
    var data = await _repository.GetClassMastery(
        query.UserId,
        query.HeroClassId,
        ct);

    return data is null
        ? null
        : _mapper.Map<UserClassMastery, UserClassMasteryDto>(data);
  }
}