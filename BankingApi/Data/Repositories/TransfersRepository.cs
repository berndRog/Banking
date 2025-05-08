using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
[assembly: InternalsVisibleTo("BankingApiTest")]
namespace BankingApi.Data.Repositories;
internal class TransfersRepository(
   DataContext dataContext
): ABaseRepository<Transfer, Guid>(dataContext), ITransfersRepository {

   public async Task<IEnumerable<Transfer>> SelectByAccountIdAsync(
      Guid accountId,
      CancellationToken ctToken = default
   ) {
      var transfers = await _dbSet
         .Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.AccountId == accountId)
         .ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransfersRepository)}: SelectByAccountIdAsync");
      return  transfers;    
   }

   public async Task<IEnumerable<Transfer>> SelectByBeneficiaryIdAsync(
      Guid beneficiaryId,
      CancellationToken ctToken = default
   ) {
      var transfers = await _dbSet
         .Include(transfer => transfer.Transactions)
         .Where(transfer => transfer.BeneficiaryId == beneficiaryId)
         .ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{nameof(TransfersRepository)}: SelectByBeneficiaryIdAsync");
      return transfers;
   }
}