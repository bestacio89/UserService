using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Users;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Users;
using UserService.Domain.Users;

namespace UserService.Application.Queries.Users;

public sealed class GetUserByUsernameQueryHandler
    : IQueryHandler<GetUserByUsernameQuery, UserDto?>
{
  private readonly IUserLookupRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetUserByUsernameQueryHandler(
      IUserLookupRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<UserDto?> Handle(GetUserByUsernameQuery query, CancellationToken ct)
  {
    var user = await _repository.GetByUsernameAsync(query.Username, ct);

    return user is null
        ? null
        : _mapper.Map<User, UserDto>(user);
  }
}