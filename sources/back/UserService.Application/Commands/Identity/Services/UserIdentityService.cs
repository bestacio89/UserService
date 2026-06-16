using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using UserService.Domain.Identity;
using UserService.Contracts.Commands.Identity;

namespace UserService.Application.Commands.Identity.Services;

public sealed class UserIdentityService : IUserIdentityService
{
  private readonly IEntityFactory<Guid, UserIdentity> _identityFactory;
  private readonly IEntityRepository<UserIdentity, Guid> _identities;

  public UserIdentityService(
      IEntityFactory<Guid, UserIdentity> identityFactory,
      IEntityRepository<UserIdentity, Guid> identities)
  {
    _identityFactory = identityFactory;
    _identities = identities;
  }

  public async Task<Guid> LinkAsync(
      LinkUserIdentityCommand command,
      CancellationToken cancellationToken)
  {
    var identity = _identityFactory.Create();

    identity.Define(
        command.UserId,
        Enum.Parse<IdentityProvider>(command.Provider, ignoreCase: true),
        command.ExternalId,
        command.DeviceId
    );

    await _identities.AddAsync(identity, cancellationToken);

    return identity.Id;
  }
}