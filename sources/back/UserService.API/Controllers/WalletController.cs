using Franz.Common.Mediator.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using UserService.Contracts.Commands.Wallet;
using UserService.Contracts.DTOs.Wallet;
using UserService.Contracts.Queries.Wallet;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/wallet")]
public sealed class WalletController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public WalletController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // GET /api/wallet/{userId} — tous les soldes du joueur, une entrée par monnaie
  [HttpGet("{userId:guid}")]
  public Task<IReadOnlyList<UserWalletBalanceDto>> GetWallet(Guid userId, CancellationToken ct)
      => _dispatcher.SendAsync(new GetUserWalletQuery(userId), ct);

  // GET /api/wallet/currencies — catalogue des monnaies actives (pour le futur Shop)
  [HttpGet("currencies")]
  public Task<IReadOnlyList<CurrencyDto>> GetCurrencies(CancellationToken ct)
      => _dispatcher.SendAsync(new GetActiveCurrenciesQuery(), ct);

  // POST /api/wallet/{userId}/credit
  [HttpPost("{userId:guid}/credit")]
  public Task<bool> Credit(Guid userId, CreditCurrencyCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = userId }, ct);

  // POST /api/wallet/{userId}/debit
  [HttpPost("{userId:guid}/debit")]
  public Task<bool> Debit(Guid userId, DebitCurrencyCommand command, CancellationToken ct)
      => _dispatcher.SendAsync(command with { UserId = userId }, ct);
}