using Microsoft.EntityFrameworkCore;
using UserService.Application.ReadModels.Progression;
using UserService.Domain.Progression;


namespace UserService.Persistence.Repositories;

public sealed class UserProgressionLookupRepository : IUserProgressionLookupRepository
{
  private readonly ApplicationDbContext _dbContext;

  public UserProgressionLookupRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<IQueryable<UserHeroMastery>> GetHeroMasteries(Guid userId, CancellationToken ct)
  {
    return _dbContext.UserHeroMasteries
        .AsNoTracking()
        .Where(x => x.UserId == userId)
        .ToListAsync(ct) ;
  }

  public Task<UserHeroMastery?> GetHeroMastery(Guid userId, Guid heroId, CancellationToken ct)
  {
    return _dbContext.UserHeroMasteries
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserId == userId && x.HeroId == heroId, ct);
  }

  public Task<UserClassMastery?> GetClassMastery(Guid userId, Guid heroClassId, CancellationToken ct)
  {
    return _dbContext.UserClassMasteries
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.UserId == userId && x.HeroClassId == heroClassId, ct);
  }
}