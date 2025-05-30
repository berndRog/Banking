using System.Linq.Expressions;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
namespace BankingApi.Core; 
public interface ITransactionsRepository: IBaseRepository<Transaction,Guid> {
   Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   );
   
   Task<IEnumerable<TransactionListItemDto>> FilterListItemsByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   );
}