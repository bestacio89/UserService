using UserService.Application.ReadModels.Ranked;
using UserService.Contracts.DTOs.Ranked;
using UserService.Domain.Ranked;


namespace UserService.Persistence.Repositories;

public sealed class RankedLookupRepository : IRankedLookupRepository
{
  private readonly ApplicationDbContext _dbContext;

  public RankedLookupRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<UserRank?> GetRankAsync(Guid userId, CancellationToken ct)
  {
    return _dbContext.UserRanks
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserId == userId, ct);
  }

  public async Task<RankedEligibilityResponseDto> CheckEligibility(
    Guid userId,
    Guid heroClassId,
    CancellationToken ct)
  {
    var mastery = await _dbContext.UserClassMasteries
        .AsNoTracking()
        .FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.HeroClassId == heroClassId,
            ct);

    if (mastery is null)
    {
      return new RankedEligibilityResponseDto
      {
        CanQueueRanked = false,
        Reason = "No mastery found for this hero class"
      };
    }

    if (mastery.MasteryLevel < 30)
    {
      return new RankedEligibilityResponseDto
      {
        CanQueueRanked = false,
        Reason = "Mastery level too low (minimum 30 required)"
      };
    }

    return new RankedEligibilityResponseDto
    {
      CanQueueRanked = true,
      Reason = null
    };


  }
}