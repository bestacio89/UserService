// Queries/Books/GetBookByTitleQuery.cs
using UserService.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace UserService.Contracts.Queries.Books
{
    public sealed record GetBookByTitleQuery(string Title) : IQuery<Result<IEnumerable<BookDto>>>;
}


