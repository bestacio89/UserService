// Application/Books/Queries/ListBooksQuery.cs
using UserService.Contracts.DTOs;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;

namespace UserService.Contracts.Queries.Books;

public sealed record ListBooksQuery
    : IQuery<Result<IReadOnlyCollection<BookDto>>>;


