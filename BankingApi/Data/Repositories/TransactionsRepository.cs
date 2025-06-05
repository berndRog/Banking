using System.Linq.Expressions;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;
public class TransactionsRepository(
   DataContext dataContext
): ABaseRepository<Transaction,Guid>(dataContext), ITransactionsRepository {
   
   public async Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet  // IQueryable<Transaction>
         .Where(t => t.AccountId == accountId);
      if (predicate is not null)
         query = query.Where(predicate);
      var transactions =  await query.ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransactionsRepository)}: FilterByAccountIdAsync");
      return transactions;
   }
   
   public async Task<IEnumerable<TransactionListItemDto>> FilterListItemsByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable()
         .Where(t => t.AccountId == accountId);
      
      if (predicate is not null) 
         query = query.Where(predicate);

      var dtoQuery = query                           // IQueryable<Transaction>
         .Include(ta => ta.Transfer)                 // Include Transfers
            .ThenInclude(tf => tf.Beneficiary)       // Include Beneficiary
         .Select(ta => new TransactionListItemDto(
            // Id, Date and Amount of Transaction
            ta.Id,            
            ta.Date,
            ta.Amount,                           
            // Description of transaction.Transfer
            ta.Transfer == null ? "" : ta.Transfer.Description,  // description of transfer
            // Name and Iban of transaction.Transfer.Beneficiary
            ta.Transfer != null && ta.Transfer.Beneficiary != null ? ta.Transfer.Beneficiary.Name : "",
            ta.Transfer != null && ta.Transfer.Beneficiary != null ? ta.Transfer.Beneficiary.Iban : "",
            // AccountId and TransferdId of Transaction
            ta.AccountId,
            ta.TransferId ?? Guid.Empty
         ));
   
      var transactions = await dtoQuery.ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(Transaction)}: FilterListItemsByAccountIdAsync");
      return transactions;
   }
   
}