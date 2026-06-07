using UserService.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace UserService.Contracts.Queries.Books
{
    // Ensure the generic parameter matches the handler's expected response type
    public sealed record GetBooksByAuthorQuery(string Author) : IQuery<Result<IEnumerable<BookDto>>>;
}

