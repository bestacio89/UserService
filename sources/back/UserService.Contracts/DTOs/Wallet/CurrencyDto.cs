namespace UserService.Contracts.DTOs.Wallet;

public sealed record CurrencyDto
{
  public Guid Id { get; init; }
  public string Code { get; init; } = string.Empty;
  public string DisplayName { get; init; } = string.Empty;
  public bool IsPremium { get; init; }
}