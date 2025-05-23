using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface ITransfersRepository: IBaseRepository<Transfer,Guid> {
   
   Task<IEnumerable<Transfer>> FilterByAccountIdJoinTransactionsAsync(
      Guid accountId,
      CancellationToken ctToken = default
   );
   Task<IEnumerable<Transfer>> FilterByBeneficiaryIdJoinTransactionsAsync(
      Guid beneficiaryId,
      CancellationToken ctToken = default
   );
   
}