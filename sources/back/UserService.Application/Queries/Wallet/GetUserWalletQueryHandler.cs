using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Contracts.DTOs.Wallet;
using UserService.Contracts.Persistence;
using UserService.Contracts.Queries.Wallet;
using UserService.Domain.Wallet;

namespace UserService.Application.Queries.Wallet;

public sealed class GetUserWalletQueryHandler
    : IQueryHandler<GetUserWalletQuery, IReadOnlyList<UserWalletBalanceDto>>
{
  private readonly IUserWalletLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetUserWalletQueryHandler(
      IUserWalletLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<UserWalletBalanceDto>> Handle(GetUserWalletQuery query, CancellationToken ct)
  {
    var balances = await _repository.GetBalancesAsync(query.UserId, ct);

    return balances
        .Select(x => _mapper.Map<UserWalletBalance, UserWalletBalanceDto>(x))
        .ToList();
  }
}