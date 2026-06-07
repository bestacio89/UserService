

using Franz.Common.Business.Domain;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using UserService.Contracts.DTOs;
using UserService.Domain.Entities;

using UserService.Contracts.Queries.Members;
using UserService.Persistence;
using Franz.Common.EntityFramework.Repositories;

namespace UserService.Application.Members.Queries;

public sealed class ListMembersQueryHandler
    : IQueryHandler<ListMembersQuery, Result<IReadOnlyCollection<MemberDto>>>
{
    private readonly EntityRepository<ApplicationDbContext, Member, int> _memberRepository;
    private readonly IFranzMapper _mapper;

    public ListMembersQueryHandler(EntityRepository<ApplicationDbContext, Member, int> memberRepository, IFranzMapper mapper)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<MemberDto>>> Handle(ListMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _memberRepository.GetAll(cancellationToken);
        var mapped = _mapper.Map<IReadOnlyCollection<Member>,IReadOnlyCollection<MemberDto>>((IReadOnlyCollection<Member>)members);

        return mapped.ToResult();
    }
}


