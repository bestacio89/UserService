using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Dispatchers;
using UserService.Contracts.Commands.Users;
using UserService.Domain.Identity;
using UserService.Domain.Ranked;
using UserService.Domain.Users;

namespace UserService.Application.Commands.Users.Services;

public sealed class UserCreationService : IUserCreationService
{
  private readonly IEntityFactory<Guid, User> _userFactory;
  private readonly IEntityFactory<Guid, UserRank> _rankFactory;
  private readonly IEntityFactory<Guid, UserIdentity> _identityFactory;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEntityRepository<User, Guid> _users;
  private readonly IEntityRepository<UserRank, Guid> _ranks;
  private readonly IEntityRepository<UserIdentity, Guid> _identities;

  public UserCreationService(
      IEntityFactory<Guid, User> userFactory,
      IEntityFactory<Guid, UserRank> rankFactory,
      IEntityFactory<Guid, UserIdentity> identityFactory,
      IUnitOfWork unitOfWork,
      IEntityRepository<User, Guid> users,
      IEntityRepository<UserRank, Guid> ranks,
      IEntityRepository<UserIdentity, Guid> identities)
  {
    _userFactory = userFactory;
    _rankFactory = rankFactory;
    _identityFactory = identityFactory;

    _users = users;
    _ranks = ranks;
    _identities = identities;
  }

  public async Task<Guid> CreateAsync(
      CreateUserCommand request,
      CancellationToken cancellationToken)
  {
    var externalUserId = MediatorContext.Current.UserId ?? "system";

    // =====================================================
    // 1. Create User (core identity)
    // =====================================================
    var user = _userFactory.Create();
    user.Define(request.Username);

    // =====================================================
    // 2. Initialize Ranked state
    // =====================================================
    var rank = _rankFactory.Create();
    rank.ApplyMatchResult(
        win: false,
        mmrDelta: 0
    );

    // =====================================================
    // 3. Create Identity (FACTORY FIXED)
    // =====================================================
    var identity = _identityFactory.Create();

    identity.Define(
        user.Id,
        IdentityProvider.Guest,
        externalUserId,
        deviceId: null
    );

    // =====================================================
    // 4. Persist
    // =====================================================
    await _users.AddAsync(user, cancellationToken);
    await _ranks.AddAsync(rank, cancellationToken);
    await _identities.AddAsync(identity, cancellationToken);
    await _unitOfWork.CommitAsync(cancellationToken);
    return user.Id;
  }
}