using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IAccountsRepository: IBaseRepository<Account,Guid> {

   Task<IEnumerable<Account>> SelectByOwnerIdAsync(
      Guid ownerId,
      CancellationToken ctToken = default 
   );
      
   Task<Account?> FindByIdJoinAsync(
      Guid id,
      bool joinOwner = false, 
      bool joinBeneficiaries = false,
      bool joinTransfers = false,
      bool joinTransactions = false,
      CancellationToken ctToken = default
   );

}