using Franz.Common.Mapping.Profiles;
using UserService.Contracts.DTOs.Wallet;
using UserService.Domain.Wallet;

namespace UserService.Application.Mappings.Wallet;

public sealed class WalletProfile : FranzMapProfile
{
  public WalletProfile()
  {
    // =========================================================
    // UserWalletBalance → DTO
    // =========================================================
    CreateMap<UserWalletBalance, UserWalletBalanceDto>()
        .ConstructUsing(src => new UserWalletBalanceDto
        {
          CurrencyCode = src.CurrencyCode,
          CurrencyDisplayName = src.CurrencyDisplayName,
          Amount = src.Amount
        });

    // =========================================================
    // Currency → DTO
    // =========================================================
    CreateMap<Currency, CurrencyDto>()
        .ConstructUsing(src => new CurrencyDto
        {
          Id = src.Id,
          Code = src.Code,
          DisplayName = src.DisplayName,
          IsPremium = src.IsPremium
        });
  }
}