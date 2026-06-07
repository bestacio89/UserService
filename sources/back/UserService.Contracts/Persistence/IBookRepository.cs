using Franz.Common.Business.Domain;
using Franz.Common.DependencyInjection;
using UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Contracts.Persistence;
public interface IBookRepository : IScopedDependency

{
  Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
  Task<IReadOnlyList<Book>> GetBooksByAuthorAsync(string Author, CancellationToken cancellationToken = default);
  Task<IReadOnlyList<Book>> GetBooksByTitleAsync(string title, CancellationToken cancellationToken = default);
}

