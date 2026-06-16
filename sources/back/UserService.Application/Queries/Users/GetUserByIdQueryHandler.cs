using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using UserService.Application.ReadModels.Users;
using UserService.Contracts.DTOs.Users;
using UserService.Contracts.Queries.Users;
using UserService.Domain.Users;

namespace UserService.Application.Queries.Users;

public sealed class GetUserByIdQueryHandler
    : IQueryHandler<GetUserByIdQuery, UserDto?>
{
  private readonly IEntityRepository<User, Guid>_repository;
  private readonly IFranzMapper _mapper;

  public GetUserByIdQueryHandler(
      IEntityRepository<User, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<UserDto?> Handle(GetUserByIdQuery query, CancellationToken ct)
  {
    var user = await _repository.GetByIdAsync(query.UserId, ct);

    return user is null
        ? null
        : _mapper.Map<User, UserDto>(user);
  }
}