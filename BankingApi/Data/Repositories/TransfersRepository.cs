using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;
public class TransfersRepository(
   DataContext dataContext
): ABaseRepository<Transfer, Guid>(dataContext), ITransfersRepository {

   public async Task<IEnumerable<Transfer>> FilterByAccountIdJoinTransactionsAsync(
      Guid accountId,
      CancellationToken ctToken = default
   ) {
      var transfers = await _dbSet
         .Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.AccountId == accountId)
         .ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransfersRepository)}: FilterByAccountIdJoinTransactionsAsync");
      return  transfers;    
   }

   public async Task<IEnumerable<Transfer>> FilterByBeneficiaryIdJoinTransactionsAsync(
      Guid beneficiaryId,
      CancellationToken ctToken = default
   ) {
      var transfers = await _dbSet
         .Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.BeneficiaryId == beneficiaryId)
         .ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransfersRepository)}: FilterByBeneficiaryIdJoinTransactionsAsync");
      return transfers;
   }
}