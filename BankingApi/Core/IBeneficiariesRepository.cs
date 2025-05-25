using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core; 
public interface IBeneficiariesRepository: IBaseRepository<Beneficiary,Guid> {
   Task<IEnumerable<Beneficiary>> SelectByNameAsync(
      string name,
      CancellationToken ctToken = default
   );
   Task<IEnumerable<Beneficiary>> SelectByAccountIdAsync(
      Guid accountId, 
      CancellationToken ctToken = default
   );

}
