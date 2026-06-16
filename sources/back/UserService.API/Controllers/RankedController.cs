using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using UserService.Contracts.DTOs.Ranked;
using UserService.Contracts.Queries.Ranked;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/ranked")]
public sealed class RankedController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public RankedController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  [HttpGet("{userId:guid}")]
  public Task<UserRankDto> GetRank(Guid userId, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserRankQuery(userId), ct);

  [HttpGet("{userId:guid}/eligibility/{heroClassId:guid}")]
  public Task<RankedEligibilityResponseDto> CheckEligibility(
      Guid userId,
      Guid heroClassId,
      CancellationToken ct)
      => _dispatcher.SendAsync(new CheckRankedEligibilityQuery(userId, heroClassId), ct);
}