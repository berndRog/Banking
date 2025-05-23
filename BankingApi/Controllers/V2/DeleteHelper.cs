using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Controllers.V2;

public class DeleteHelper(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionRepository,
   IDataContext dataContext
) {
   
   public async Task DeleteOwnerAsync(Owner owner, CancellationToken ctToken) {
      
      // Workaround cascading delete
      var accounts = await accountsRepository
         .FilterByAsync(a => a.OwnerId == owner.Id, ctToken);

      foreach (var account in accounts) {
         await DeleteAccountAsync(account, ctToken);
      }
      
      // remove owner in repository 
      ownersRepository.Remove(owner);
      // write to database
      await dataContext.SaveAllChangesAsync("Remove Owner", ctToken);
      
   } 
   
   public async Task DeleteAccountAsync(Account account, CancellationToken ctToken) {
      
      // get all transactions for account      
      var transactions = 
         await transactionRepository.FilterByAccountIdAsync(account.Id, null, ctToken);
      // remove in repository
      foreach (var transaction in transactions)
         transactionRepository.Remove(transaction);  
      
      accountsRepository.Remove(account);
      // write to database
      await dataContext.SaveAllChangesAsync(null, ctToken);
      
   }
   
}