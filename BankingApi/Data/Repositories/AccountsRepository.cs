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
      bool joinOwner,
      bool joinBeneficiaries,
      bool joinTransfers,
      bool joinTransactions,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable();  // IQueryable<Account>
      query = query.Where(a => a.Id == id);     
      if (joinOwner) query = query.Include(a => a.Owner);
      if (joinBeneficiaries) query = query.Include(a => a.Beneficiaries);
      if (joinTransfers) query = query.Include(a => a.Transfers).ThenInclude(t => t.Transactions);
      if (joinTransactions) query = query.Include(a => a.Transactions);
      query = query.AsSplitQuery();
      var accounts =  await query.FirstOrDefaultAsync(ctToken);
      _dataContext.LogChangeTracker("{nameOf(Account).Name}: FindByIdJoinAsync");
      return accounts;

   }
   

}