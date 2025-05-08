using System.Linq.Expressions;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 

public interface IBaseRepository<T, in TId> where T : class, IEntity<TId> {
   
   // read from database?
   Task<T?> FindByIdAsync(
      TId id,
      CancellationToken ctToken = default
   );
   Task<IEnumerable<T>> SelectAsync  (
      bool withTracking = false,
      CancellationToken ctToken = default
   );
   Task<T?> FindByAsync  (
      Expression<Func<T, bool>> predicate,
      CancellationToken ctToken = default
   ); 
   Task<IEnumerable<T>> FilterByAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken ctToken = default
   );
   
   // write to in-memory repository
   void Add(T entity);
   void AddRange (IEnumerable<T> entities);
   void Update(T entity);
   void Remove(T entity);
}