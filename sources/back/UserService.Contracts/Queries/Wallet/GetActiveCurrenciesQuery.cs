using Franz.Common.Mediator.Messages;
using UserService.Contracts.DTOs.Wallet;

namespace UserService.Contracts.Queries.Wallet;

public sealed record GetActiveCurrenciesQuery() : IQuery<IReadOnlyList<CurrencyDto>>;