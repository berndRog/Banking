using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;
public class BeneficiariesRepository(
   DataContext dataContext
) : ABaseRepository<Beneficiary, Guid>(dataContext), IBeneficiariesRepository {
   
   public async Task<IEnumerable<Beneficiary>> SelectByNameAsync(
      string name,
      CancellationToken ctToken = default
   ) {
      var entities = await _dbSet
         .Where(o => EF.Functions.Like(o.Name, "%"+name.Trim()+"%"))
         .ToListAsync(ctToken); 
      _dataContext.LogChangeTracker($"{nameof(Beneficiary)}: SelectByNameAsync");
      return entities;
   }

   public async Task<IEnumerable<Beneficiary>> SelectByAccountIdAsync(
      Guid accountId,
      CancellationToken ctToken = default
   ){
      var entities = await _dbSet
         .Where(b => b.AccountId == accountId)
         .ToListAsync(ctToken);  
      _dataContext.LogChangeTracker($"{nameof(Beneficiary)}: SelectByAccountIdAsync");
      return entities;
   }
}