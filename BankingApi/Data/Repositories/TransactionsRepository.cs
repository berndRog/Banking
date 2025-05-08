using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
using Microsoft.EntityFrameworkCore;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Data.Repositories;
internal class TransactionsRepository(
   DataContext dataContext
): ABaseRepository<Transaction,Guid>(dataContext), ITransactionsRepository {
   
   public async Task<IEnumerable<Transaction>> FilterByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable()  // IQueryable<Transaction>
         .Where(t => t.AccountId == accountId).AsQueryable()
         .AsSplitQuery();
      if(predicate is not null) query = query
         .Where(predicate)
         .AsSingleQuery();
      var transactions =  await query.ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransactionsRepository)}: FilterByAccountIdAsync");
      return transactions;
   }

   public async Task<IEnumerable<Transaction>> SelectByTransferIdAsync(
      Guid transferId,
      CancellationToken ctToken = default
   ){
      var transactions = await _dbSet
         .Where(transaction => transaction.TransferId == transferId)
         .ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransactionsRepository)}: SelectByTransferIdAsync");
      return transactions;
   }
   
   
   public async Task<IEnumerable<TransactionListItemDto>> FilterListItemsByAccountIdAsync(
      Guid accountId,
      Expression<Func<Transaction, bool>>? predicate,
      CancellationToken ctToken = default
   ) {
      // transaction for AccountId
      var query = _dbSet.AsQueryable() // IQueryable<Transaction>
         .Where(t => t.AccountId == accountId);
      if (predicate is not null) 
         query = query.Where(predicate);

      // transaction --> transfer --> beneficiary
      var dtoQuery = query  // (t = transaction)
         .Include(t => t.Transfer)
         .Where(t => t.Transfer != null)  // Filter out null transfers before ThenInclude
         .Include(t => t.Transfer!.Beneficiary)
         .Select(t => new TransactionListItemDto(
            t.Id,
            t.Date,
            t.Amount,
            t.Transfer == null ? "" : t.Transfer.Description,
            t.Transfer != null && t.Transfer.Beneficiary != null ? t.Transfer.Beneficiary.Name : "",
            t.Transfer != null && t.Transfer.Beneficiary != null ? t.Transfer.Beneficiary.Iban : "",
            t.AccountId,
            t.TransferId ?? Guid.Empty
         ));
      
      var transactions = await dtoQuery.ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(Transaction)}: FilterListItemsByAccountIdAsync");
      return transactions;
   }
}