using UserService.Contracts.Commands.Identity;
using UserService.Client.Http.Abstractions;

namespace UserService.Client.Http.Clients;

public sealed class IdentityClient : HttpClientBase
{
  public IdentityClient(HttpClient http) : base(http) { }

  public Task<Guid> CreateGuest(CreateGuestIdentityCommand command, CancellationToken ct)
      => PostAsync<CreateGuestIdentityCommand, Guid>("/api/identity/guest", command, ct);

  public Task<Guid> Link(LinkUserIdentityCommand command, CancellationToken ct)
      => PostAsync<LinkUserIdentityCommand, Guid>("/api/identity/link", command, ct);
}