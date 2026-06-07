using Franz.Common.Business.Repositories;
using Franz.Common.EntityFramework.Repositories;
using Franz.Common.Mediator.Errors;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using UserService.Contracts.Commands.Books;
using UserService.Domain.Entities;
using UserService.Persistence;


namespace UserService.Application.Books.Commands
{
    public sealed class BorrowBookCommandHandler : ICommandHandler<BorrowBookCommand, Result>
    {
        private readonly EntityRepository<ApplicationDbContext, Book, int> _bookRepository;
        private readonly EntityRepository<ApplicationDbContext, Member, int> _memberRepository;

        public BorrowBookCommandHandler(EntityRepository<ApplicationDbContext, Book, int> bookRepository, EntityRepository<ApplicationDbContext, Member, int> memberRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
        }

        public async Task<Result> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.BookId, cancellationToken);
            if (book is null)
                return Result.Failure(Error.NotFound("Book", request.BookId));

            var member = await _memberRepository.GetByIdAsync(request.MemberId, cancellationToken);
            if (member is null)
                return Result.Failure(Error.NotFound("Member", request.MemberId));

            var borrowResult = member.BorrowBook(book);
            if (borrowResult.IsSuccess)
                return borrowResult;

            await _bookRepository.UpdateAsync(book, cancellationToken);
            await _memberRepository.UpdateAsync(member,cancellationToken);

            return Result.Success();
        }
    }

}


