using Microsoft.EntityFrameworkCore;
using UserService.Application.ReadModels.Users;
using UserService.Domain.Users;


namespace UserService.Persistence.Repositories;

public sealed class UserLookupRepository : IUserLookupRepository
{
  private readonly ApplicationDbContext _dbContext;

  public UserLookupRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<User?> GetByIdAsync(Guid userId, CancellationToken ct)
  {
    return _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId, ct);
  }

  public Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
  {
    return _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Username == username, ct);
  }

  public Task<User?> GetAdminProfileAsync(Guid userId, CancellationToken ct)
  {
    return _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == userId, ct);
  }

  public Task<UserAccountState?> GetAccountStateAsync(Guid userId, CancellationToken ct)
  {
    return _dbContext.Users
        .AsNoTracking()
        .Where(u => u.Id == userId)
        .Select(u => (UserAccountState?)u.State)
        .FirstOrDefaultAsync(ct);
  }
}