using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Ranked;
using UserService.Contracts.DTOs.Ranked;
using UserService.Contracts.Queries.Ranked;

namespace UserService.Application.Queries.Ranked;

public sealed class CheckRankedEligibilityQueryHandler
    : IQueryHandler<CheckRankedEligibilityQuery, RankedEligibilityResponseDto>
{
  private readonly IRankedLookupRepository _repository;

  public CheckRankedEligibilityQueryHandler(IRankedLookupRepository repository)
  {
    _repository = repository;
  }

  public Task<RankedEligibilityResponseDto> Handle(
      CheckRankedEligibilityQuery query,
      CancellationToken ct)
  {
    return _repository.CheckEligibility(query.UserId, query.HeroClassId, ct);
  }
}