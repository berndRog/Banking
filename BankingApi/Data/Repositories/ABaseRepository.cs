using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApi.Data.Repositories;

public abstract class ABaseRepository<T, TId>(
   DataContext dataContext
) : IBaseRepository<T, TId> where T : class, IEntity<TId> {
   // fields
   protected readonly DataContext _dataContext = dataContext;
   protected readonly DbSet<T> _dbSet = dataContext.Set<T>();

   // virtual methods, can be overridden in derived classes
   public virtual async Task<T?> FindByIdAsync(
      TId id,
      CancellationToken ctToken = default
   ) {
      object[] keyValues = [ id ];
      var entity = await _dbSet.FindAsync(keyValues, ctToken);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: FindByIdAsync");
      return entity;
   }

   public virtual async Task<IEnumerable<T>> SelectAsync(
      bool withTracking = false,
      CancellationToken ctToken = default
   ) {
      IQueryable<T> query = _dbSet;
      if (!withTracking) query = query.AsNoTracking();
      var items = await query.ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: SelectAllAsync");
      return items;
   }

   // Find an item by LINQ expression asynchronously
   public virtual async Task<T?> FindByAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken ctToken = default
   ) {
      var entity = await _dbSet.FirstOrDefaultAsync(predicate, ctToken);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: FindByAsync");
      return entity;
   }
   
   // Filter items by LINQ expression asynchronously
   public virtual async Task<IEnumerable<T>> FilterByAsync(
      Expression<Func<T, bool>> predicate,
      CancellationToken ctToken = default
   ) {
      var entities = await _dbSet.Where(predicate).ToListAsync(ctToken);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: FilterByAsync");
      return entities;
   }
   
   public virtual void Add(T entity) {
      _dbSet.Add(entity);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: Add");
   }

   public virtual void AddRange(IEnumerable<T> entities) {
      _dbSet.AddRange(entities);
      _dataContext.LogChangeTracker($"{typeof(T).Name}: AddRange");
   }

   public virtual void Update(T entity) {
      var existingEntity = _dbSet.Find(entity.Id);
      if (existingEntity == null)
         throw new ApplicationException($"Update failed, entity with given id not found");
      var entry = _dataContext.Entry(existingEntity);
      if (entry.State == EntityState.Detached) _dbSet.Attach(entity);
      entry.State = EntityState.Modified;
   }

   public void Remove(T entity) {
      var existingEntity = _dbSet.Find(entity.Id);
      if (existingEntity == null)
         throw new ApplicationException($"Remove failed, entity with given id not found");
      var entry = _dataContext.Entry(existingEntity);
      if (entry == null) throw new Exception($"{typeof(T).Name} to be removed not found");
      if (entry.State == EntityState.Detached) _dbSet.Attach(entity);
      entry.State = EntityState.Deleted;
   }
   
}