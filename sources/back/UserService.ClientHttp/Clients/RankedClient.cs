using UserService.Contracts.DTOs.Ranked;
using UserService.Contracts.Queries.Ranked;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class RankedClient : HttpClientBase
{
  public RankedClient(HttpClient http) : base(http) { }

  public Task<UserRankDto> GetRank(Guid userId, CancellationToken ct)
      => GetAsync<UserRankDto>($"/api/ranked/{userId}", ct);

  public Task<RankedEligibilityResponseDto> CheckEligibility(
      Guid userId,
      Guid heroClassId,
      CancellationToken ct)
      => GetAsync<RankedEligibilityResponseDto>(
          $"/api/ranked/{userId}/eligibility/{heroClassId}",
          ct);
}