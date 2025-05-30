using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data.Repositories;
public class OwnersRepository(
   DataContext dataContext
) : ABaseRepository<Owner,Guid>(dataContext), IOwnersRepository { 

   public async Task<IEnumerable<Owner>> SelectByNameAsync(
      string name,
      CancellationToken ctToken = default
   ) {
      var entities = await _dbSet
         .Where(o => EF.Functions.Like(o.Name, "%"+name.Trim()+"%"))
         .ToListAsync(ctToken); 
      _dataContext.LogChangeTracker($"{nameof(Owner)}: SelectByNameAsync");
      return entities;
   }
   
   public async Task<Owner?> FindByIdJoinAsync(
      Guid id, 
      bool join,
      CancellationToken ctToken = default
   ){
      var query = _dbSet.AsQueryable(); // IQueryable<Owner>
      if (join) query = query
         .Include(o => o.Accounts).ThenInclude(a => a.Beneficiaries)
         .Include(o => o.Accounts).ThenInclude(a => a.Transfers).ThenInclude(t => t.Transactions)
         .Include(o => o.Accounts).ThenInclude(a => a.Transfers).ThenInclude(t => t.Beneficiary)
         .Include(o => o.Accounts).ThenInclude(a => a.Transactions)
         .AsSingleQuery();
      var owner = await query.FirstOrDefaultAsync(o => o.Id == id, ctToken);
      _dataContext.LogChangeTracker($"{nameof(Owner)}: FindbyIdJoineAsync");
      return owner;
   }
   
   public async Task<Owner?> FindByJoinAsync(
      Expression<Func<Owner, bool>> predicate,
      bool join,
      CancellationToken ctToken = default
   ) {
      var query = _dbSet.AsQueryable(); // IQueryable<Owner>
      if (join) query = query
         .Include(o => o.Accounts).ThenInclude(a => a.Beneficiaries)
         .Include(o => o.Accounts).ThenInclude(a => a.Transfers).ThenInclude(t => t.Transactions)
         .Include(o => o.Accounts).ThenInclude(a => a.Transfers).ThenInclude(t => t.Beneficiary)
         .Include(o => o.Accounts).ThenInclude(a => a.Transactions)
         .AsSingleQuery();
      var owner = await query.FirstOrDefaultAsync(predicate, ctToken);
      _dataContext.LogChangeTracker($"{nameof(Owner)}: FindByJoinAsync");

      return owner;
   }
   
   // When applying Include to load related data, you can add enumerable operations
   // to the included collection navigation, which allows for filtering and sorting
   // of the results.
   // Supported operations are: Where, OrderBy, OrderByDescending,
   // ThenBy, ThenByDescending, Skip, and Take.
   // SELECT owner.*, accounts.*
   // FROM(
   //    SELECT *
   //    FROM Owners AS o
   //    WHERE o.Id = id
   //    LIMIT 1
   // ) AS owner
   // LEFT JOIN(
   //   SELECT *
   //   FROM Accounts AS "a"
   //   WHERE a.OwnerId = id
   // ) AS accounts
   // ON owner.Id = accounts.OwnerId
   // public async Task<Owner?> FindByIdJoin2Async(
   //    Guid id,
   //    bool joinAccounts,
   //    CancellationToken ctToken = default
   // ) {
   //    var query = _dbSet.AsQueryable();
   //    // Filtered include
   //    if (joinAccounts) {
   //       query = query.Include(owner => 
   //             owner.Accounts.Where(account => account.OwnerId == id));
   //    }
   //    var owners = await query.FirstOrDefaultAsync(o => o.Id == id, ctToken);
   //    _dataContext.LogChangeTracker($"{nameof(Owner)}: FindByIdJoin2Async");
   //    return owners;
   // }
}