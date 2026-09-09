using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework;
using Franz.Common.Mediator.Context;
using Franz.Common.Mediator.Dispatchers;
using Franz.Common.Mediator.Pipelines.Core;
using Microsoft.Extensions.Logging;
using UserService.Contracts.Commands.Users;
using UserService.Contracts.Persistence;
using UserService.Domain.Identity;
using UserService.Domain.Ranked;
using UserService.Domain.Users;
using UserService.Domain.Wallet;

namespace UserService.Application.Commands.Users.Services;

public sealed class UserCreationService : IUserCreationService
{
  private static readonly string[] DefaultCurrencyCodes = { "SOFT", "HARD" };

  private readonly IEntityFactory<Guid, User> _userFactory;
  private readonly IEntityFactory<Guid, UserRank> _rankFactory;
  private readonly IEntityFactory<Guid, UserIdentity> _identityFactory;
  private readonly IEntityFactory<Guid, UserWalletBalance> _walletBalanceFactory;
  private readonly ICurrencyLookupRepository _currencies;
  private readonly IUnitOfWork _unitOfWork;
  private readonly IEntityRepository<User, Guid> _users;
  private readonly IEntityRepository<UserRank, Guid> _ranks;
  private readonly IEntityRepository<UserIdentity, Guid> _identities;
  private readonly IEntityRepository<UserWalletBalance, Guid> _walletBalances;
  private readonly ILogger<UserCreationService> _logger;

  public UserCreationService(
      IEntityFactory<Guid, User> userFactory,
      IEntityFactory<Guid, UserRank> rankFactory,
      IEntityFactory<Guid, UserIdentity> identityFactory,
      IEntityFactory<Guid, UserWalletBalance> walletBalanceFactory,
      ICurrencyLookupRepository currencies,
      IUnitOfWork unitOfWork,
      IEntityRepository<User, Guid> users,
      IEntityRepository<UserRank, Guid> ranks,
      IEntityRepository<UserIdentity, Guid> identities,
      IEntityRepository<UserWalletBalance, Guid> walletBalances,
      ILogger<UserCreationService> logger)
  {
    _userFactory = userFactory;
    _rankFactory = rankFactory;
    _identityFactory = identityFactory;
    _walletBalanceFactory = walletBalanceFactory;
    _currencies = currencies;

    _unitOfWork = unitOfWork;
    _users = users;
    _ranks = ranks;
    _identities = identities;
    _walletBalances = walletBalances;
    _logger = logger;
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
    // 4. Initialize Wallet balances (SOFT + HARD, both start at zero)
    //
    // Relies on the Currency catalog already containing these codes
    // (seeded by CurrencySeeder in dev). If a code is missing — e.g. the
    // seeder hasn't run in this environment yet — we log and skip that
    // balance rather than failing account creation entirely; a missing
    // balance row is safely backfillable later, a blocked signup isn't.
    // Production needs a real Currency seed/migration story, not just
    // the dev-only TemplateDatabaseSeeder.Run() gate in Program.cs.
    // =====================================================
    var walletBalances = new List<UserWalletBalance>();

    foreach (var code in DefaultCurrencyCodes)
    {
      var currency = await _currencies.GetByCodeAsync(code, cancellationToken);

      if (currency is null)
      {
        _logger.LogWarning(
            "Currency '{CurrencyCode}' not found while creating user {UserId} — wallet balance not seeded for this currency.",
            code,
            user.Id);
        continue;
      }

      var balance = _walletBalanceFactory.Create();
      balance.Define(user.Id, currency.Id, currency.Code, currency.DisplayName);
      walletBalances.Add(balance);
    }

    // =====================================================
    // 5. Persist
    // =====================================================
    await _users.AddAsync(user, cancellationToken);
    await _ranks.AddAsync(rank, cancellationToken);
    await _identities.AddAsync(identity, cancellationToken);

    foreach (var balance in walletBalances)
    {
      await _walletBalances.AddAsync(balance, cancellationToken);
    }

    await _unitOfWork.CommitAsync(cancellationToken);
    return user.Id;
  }
}