using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IAccountsRepository: IBaseRepository<Account,Guid> {

   Task<IEnumerable<Account>> SelectByOwnerIdAsync(
      Guid ownerId,
      CancellationToken ctToken = default 
   );
   
   Task<Account?> FindByIdJoinAsync(
      Guid id,
      bool join = true,
      CancellationToken ctToken = default
   );

}