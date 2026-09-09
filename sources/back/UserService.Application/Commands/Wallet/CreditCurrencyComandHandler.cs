using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.Commands.Wallet;
using UserService.Contracts.Persistence;
using UserService.Domain.Wallet;

namespace UserService.Application.Commands.Wallet;

public sealed class CreditCurrencyCommandHandler
    : ICommandHandler<CreditCurrencyCommand, bool>
{
  private readonly ICurrencyLookupRepository _currencies;
  private readonly IUserWalletLookupRepository _balances;
  private readonly IEntityFactory<Guid, UserWalletBalance> _balanceFactory;
  private readonly IEntityRepository<UserWalletBalance, Guid> _balanceRepository;

  public CreditCurrencyCommandHandler(
      ICurrencyLookupRepository currencies,
      IUserWalletLookupRepository balances,
      IEntityFactory<Guid, UserWalletBalance> balanceFactory,
      IEntityRepository<UserWalletBalance, Guid> balanceRepository)
  {
    _currencies = currencies;
    _balances = balances;
    _balanceFactory = balanceFactory;
    _balanceRepository = balanceRepository;
  }

  public async Task<bool> Handle(
      CreditCurrencyCommand command,
      CancellationToken cancellationToken)
  {
    var currency = await _currencies.GetByCodeAsync(command.CurrencyCode, cancellationToken);

    if (currency is null || !currency.IsActive)
    {
      return false;
    }

    var balance = await _balances.GetBalanceAsync(command.UserId, currency.Id, cancellationToken);

    if (balance is null)
    {
      // Premier crédit du joueur sur cette monnaie (ex: nouveau event
      // token) — on crée la ligne à zéro plutôt que d'exiger un
      // provisionnement préalable pour chaque monnaie future.
      balance = _balanceFactory.Create();
      balance.Define(command.UserId, currency.Id, currency.Code, currency.DisplayName);
      balance.Credit(command.Amount, command.Reason);

      await _balanceRepository.AddAsync(balance, cancellationToken);
    }
    else
    {
      balance.Credit(command.Amount, command.Reason);

      await _balanceRepository.UpdateAsync(balance, cancellationToken);
    }

    return true;
  }
}