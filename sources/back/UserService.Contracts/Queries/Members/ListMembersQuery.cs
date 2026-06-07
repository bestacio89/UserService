// Application/Members/Queries/ListMembersQuery.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using UserService.Contracts.DTOs;

namespace UserService.Contracts.Queries.Members;

public sealed record ListMembersQuery
    : IQuery<Result<IReadOnlyCollection<MemberDto>>>;


