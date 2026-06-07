// Application/Members/Queries/GetMemberByIdQuery.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using UserService.Contracts.DTOs;

namespace UserService.Contracts.Queries.Members;

public sealed record GetMemberByIdQuery(int MemberId)
    : IQuery<Result<MemberDto>>;


