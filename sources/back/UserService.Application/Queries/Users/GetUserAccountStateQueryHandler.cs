using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Users;
using UserService.Contracts.Queries.Admin;
using UserService.Contracts.Queries.Users;
using UserService.Domain.Users;

namespace UserService.Application.Queries.Users;

public sealed class GetUserAccountStateQueryHandler
    : IQueryHandler<GetUserAccountStateQuery, UserAccountState?>
{
  private readonly IUserLookupRepository _repository;

  public GetUserAccountStateQueryHandler(IUserLookupRepository repository)
  {
    _repository = repository;
  }

  public Task<UserAccountState?> Handle(
      GetUserAccountStateQuery query,
      CancellationToken ct)
  {
    return _repository.GetAccountStateAsync(query.UserId, ct);
  }
}