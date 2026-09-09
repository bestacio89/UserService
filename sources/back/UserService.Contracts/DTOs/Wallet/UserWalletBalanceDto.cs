namespace UserService.Contracts.DTOs.Wallet;

public sealed record UserWalletBalanceDto
{
  public string CurrencyCode { get; init; } = string.Empty;
  public string CurrencyDisplayName { get; init; } = string.Empty;
  public int Amount { get; init; }
}