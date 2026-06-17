using Microsoft.EntityFrameworkCore;
using UserService.Application.ReadModels.Authentication;
using UserService.Contracts.Persistence;
using UserService.Domain.Identity;
using UserService.Domain.Users;

namespace UserService.Persistence.Repositories;

public sealed class UserIdentityLookupRepository : IUserIdentityLookupRepository
{
  private readonly ApplicationDbContext _dbContext;

  public UserIdentityLookupRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<UserIdentity?> GetByProviderAsync(
      string provider,
      string providerUserId,
      CancellationToken ct)
  {
    return await _dbContext.UserIdentities
        .AsNoTracking()
        .FirstOrDefaultAsync(x =>
            x.Provider == provider &&
            x.ProviderUserId == providerUserId,
            ct);
  }
}