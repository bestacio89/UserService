// Application/Members/Commands/DeleteMemberCommand.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace UserService.Contracts.Commands.Members;

public sealed record DeleteMemberCommand(int MemberId)
    : ICommand<Result>;


