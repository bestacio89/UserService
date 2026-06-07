// Queries/Members/GetMemberByNameQuery.cs

using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using UserService.Contracts.DTOs;

namespace UserService.Contracts.Queries.Members
{
    public sealed record GetMemberByNameQuery(string FullName) : IQuery<Result<MemberDto>>;
}


