using System.Collections;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;

public class AccountsRepository(
   DataContext dataContext
) : ABaseRepository<Account, Guid>(dataContext), IAccountsRepository {
   
   public async Task<IEnumerable<Account>> SelectByOwnerIdAsync(
      Guid ownerId,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable() // IQueryable<Account>
         .Where(a => a.OwnerId == ownerId);
      var accounts = await query.ToListAsync(ctToken);
      _dataContext.LogChangeTracker("{nameOf(Account).Name}: SelectByOwnerIdAsync");
      return accounts;
   }

   public async Task<Account?> FindByIdJoinAsync(
      Guid id,
      bool join,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable();  // IQueryable<Account>
      query = query.Where(a => a.Id == id);     
      if (join) query = query
         .Include(a => a.Owner)
         .Include(a => a.Beneficiaries)
         .Include(a => a.Transfers).ThenInclude(t => t.Transactions)
         .Include(a => a.Transfers).ThenInclude(t => t.Beneficiary)
         .Include(a => a.Transactions)
         .AsSingleQuery();
      var accounts =  await query.FirstOrDefaultAsync(ctToken);
      _dataContext.LogChangeTracker("{nameOf(Account).Name}: FindByIdJoinAsync");
      return accounts;

   }
}