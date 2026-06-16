using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using UserService.Contracts.Commands.Progression;
using UserService.Contracts.DTOs.Progression;
using UserService.Contracts.Queries.Progression;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/progression")]
public sealed class ProgressionController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public ProgressionController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // HERO MASTERIES
  [HttpGet("{userId:guid}/heroes")]
  public Task<IReadOnlyList<UserHeroMasteryDto>> GetHeroMasteries(Guid userId, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserHeroMasteriesQuery(userId), ct);

  [HttpGet("{userId:guid}/heroes/{heroId:guid}")]
  public Task<UserHeroMasteryDto> GetHeroMastery(Guid userId, Guid heroId, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserHeroMasteryQuery(userId, heroId), ct);

  // CLASS MASTERIES
  [HttpGet("{userId:guid}/classes/{heroClassId:guid}")]
  public Task<UserClassMasteryDto> GetClassMastery(Guid userId, Guid heroClassId, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserClassMasteryQuery(userId, heroClassId), ct);

  // COMMANDS
  [HttpPost("{userId:guid}/heroes/{heroId:guid}/match")]
  public Task<bool> RegisterMatch(
      Guid userId,
      Guid heroId,
      RegisterHeroMatchCommand command,
      CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = userId, HeroId = heroId }, ct);

  [HttpPost("{userId:guid}/classes/{heroClassId:guid}/xp")]
  public Task<bool> AddClassXp(
      Guid userId,
      Guid heroClassId,
      AddClassMasteryExperienceCommand command,
      CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = userId, HeroClassId = heroClassId }, ct);
}