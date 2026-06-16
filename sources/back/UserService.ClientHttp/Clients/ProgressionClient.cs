using UserService.Contracts.Commands.Progression;
using UserService.Contracts.DTOs.Progression;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class ProgressionClient : HttpClientBase
{
  public ProgressionClient(HttpClient http) : base(http) { }

  public Task<IReadOnlyList<UserHeroMasteryDto>> GetHeroMasteries(Guid userId, CancellationToken ct)
      => GetAsync<IReadOnlyList<UserHeroMasteryDto>>($"/api/progression/{userId}/heroes", ct);

  public Task<UserHeroMasteryDto> GetHeroMastery(Guid userId, Guid heroId, CancellationToken ct)
      => GetAsync<UserHeroMasteryDto>($"/api/progression/{userId}/heroes/{heroId}", ct);

  public Task<UserClassMasteryDto> GetClassMastery(Guid userId, Guid heroClassId, CancellationToken ct)
      => GetAsync<UserClassMasteryDto>($"/api/progression/{userId}/classes/{heroClassId}", ct);

  public Task<bool> RegisterMatch(
      Guid userId,
      Guid heroId,
      RegisterHeroMatchCommand command,
      CancellationToken ct)
      => PostAsync<RegisterHeroMatchCommand, bool>(
          $"/api/progression/{userId}/heroes/{heroId}/match",
          command,
          ct);

  public Task<bool> AddClassXp(
      Guid userId,
      Guid heroClassId,
      AddClassMasteryExperienceCommand command,
      CancellationToken ct)
      => PostAsync<AddClassMasteryExperienceCommand, bool>(
          $"/api/progression/{userId}/classes/{heroClassId}/xp",
          command,
          ct);
}