using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.DTOs.Wallet;
using UserService.Contracts.Persistence;
using UserService.Contracts.Queries.Wallet;
using UserService.Domain.Wallet;

namespace UserService.Application.Queries.Wallet;

public sealed class GetActiveCurrenciesQueryHandler
    : IQueryHandler<GetActiveCurrenciesQuery, IReadOnlyList<CurrencyDto>>
{
  private readonly ICurrencyLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetActiveCurrenciesQueryHandler(
      ICurrencyLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<CurrencyDto>> Handle(GetActiveCurrenciesQuery query, CancellationToken ct)
  {
    var currencies = await _repository.GetActiveAsync(ct);

    return currencies
        .Select(x => _mapper.Map<Currency, CurrencyDto>(x))
        .ToList();
  }
}