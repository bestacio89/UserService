using UserService.Contracts.Commands.Wallet;
using UserService.Contracts.DTOs.Wallet;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class WalletClient : HttpClientBase
{
  public WalletClient(HttpClient http) : base(http) { }

  public Task<IReadOnlyList<UserWalletBalanceDto>> GetWallet(Guid userId, CancellationToken ct)
      => GetAsync<IReadOnlyList<UserWalletBalanceDto>>($"/api/wallet/{userId}", ct);

  public Task<IReadOnlyList<CurrencyDto>> GetCurrencies(CancellationToken ct)
      => GetAsync<IReadOnlyList<CurrencyDto>>("/api/wallet/currencies", ct);

  public Task<bool> Credit(
      Guid userId,
      CreditCurrencyCommand command,
      CancellationToken ct)
      => PostAsync<CreditCurrencyCommand, bool>(
          $"/api/wallet/{userId}/credit",
          command,
          ct);

  public Task<bool> Debit(
      Guid userId,
      DebitCurrencyCommand command,
      CancellationToken ct)
      => PostAsync<DebitCurrencyCommand, bool>(
          $"/api/wallet/{userId}/debit",
          command,
          ct);
}