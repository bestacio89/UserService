// Application/Members/Commands/UpdateMemberCommand.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace UserService.Contracts.Commands.Members;

public sealed record UpdateMemberCommand(int MemberId, string FullName, string Email)
    : ICommand<Result>;


