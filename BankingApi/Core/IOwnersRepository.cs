using System.Linq.Expressions;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 

public interface IOwnersRepository: IBaseRepository<Owner,Guid> {
   Task<IEnumerable<Owner>> SelectByNameAsync(string name,
      CancellationToken ctToken = default);
   Task<Owner?> FindByIdJoinAsync(Guid id, bool join, 
      CancellationToken cancellationToken = default);
   Task<Owner?> FindByJoinAsync(Expression<Func<Owner, bool>> predicate, bool join,
      CancellationToken cancellationToken = default);
}