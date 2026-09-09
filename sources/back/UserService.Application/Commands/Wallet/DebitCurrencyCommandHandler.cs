using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.Commands.Wallet;
using UserService.Contracts.Persistence;
using UserService.Domain.Wallet;

namespace UserService.Application.Commands.Wallet;

public sealed class DebitCurrencyCommandHandler
    : ICommandHandler<DebitCurrencyCommand, bool>
{
  private readonly ICurrencyLookupRepository _currencies;
  private readonly IUserWalletLookupRepository _balances;
  private readonly IEntityRepository<UserWalletBalance, Guid> _balanceRepository;

  public DebitCurrencyCommandHandler(
      ICurrencyLookupRepository currencies,
      IUserWalletLookupRepository balances,
      IEntityRepository<UserWalletBalance, Guid> balanceRepository)
  {
    _currencies = currencies;
    _balances = balances;
    _balanceRepository = balanceRepository;
  }

  public async Task<bool> Handle(
      DebitCurrencyCommand command,
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
      // Aucun solde connu pour cette monnaie == solde de 0 == débit refusé.
      return false;
    }

    bool success = balance.Debit(command.Amount, command.Reason);

    if (!success)
    {
      // Solde insuffisant — résultat métier attendu, aucune écriture.
      return false;
    }

    await _balanceRepository.UpdateAsync(balance, cancellationToken);

    return true;
  }
}