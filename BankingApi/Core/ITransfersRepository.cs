using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface ITransfersRepository: IBaseRepository<Transfer,Guid> {
   
   Task<IEnumerable<Transfer>> SelectByAccountIdAsync(
      Guid accountId,
      CancellationToken ctToken = default
   );
   Task<IEnumerable<Transfer>> SelectByBeneficiaryIdAsync(
      Guid beneficiaryId,
      CancellationToken ctToken = default
   );
   
}