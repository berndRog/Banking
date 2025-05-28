using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core.UseCases; 
public class UseCasesTransfer(
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionsRepository,
   IDataContext dataContext
): IUseCasesTransfer {

   public async Task<ResultData<Transfer>> SendMoneyAsync(
      Guid accountDebitId,         // Account from which the transfer is made (debit) 
      Transfer transfer,           // Transfer data transferDto
      CancellationToken ctToken = default
   ){
      try {
         
         // check if debit account exists (Lastschrift)
         var accountDebit = await accountsRepository.FindByIdAsync(accountDebitId, ctToken);
         if(accountDebit == null)
            return new Error<Transfer>(404,"Debit Account for the transfer doesn't exist.");
         // overwrite transferDto with accountDebitId
         //transfer.AccountId = accountDebitId;
         transfer.SetAccount(accountDebit);
         
         // check if the benificiary exits
         if(transfer.BeneficiaryId == null) 
            return new Error<Transfer>(404, "BeneficiaryId for the transfer not given.");
         var beneficiary = 
            await beneficiariesRepository.FindByIdAsync((Guid)transfer.BeneficiaryId, ctToken);
         if(beneficiary == null) 
            return new Error<Transfer>(404, "Beneficiary for the transfer doesn't exist.");
      
         // check if credit account exists (Gutschrift)
         var accountCredit = 
            await accountsRepository.FindByAsync(a => a.Iban == beneficiary.Iban, ctToken);
         if(accountCredit == null)
            return new Error<Transfer>(404, "Credit Account (Iban) for the transfer doesn't exist."); 
         
         // check if transfer with transferDto.Id exists
         if(await transfersRepository.FindByIdAsync(transfer.Id, ctToken) != null)
            return new Error<Transfer>(409, "Transfer already exists.");
         
         Transaction transactionDebit = new (
            id: Guid.NewGuid(),
            date: transfer.Date,
            amount: -transfer.Amount,
            null,
            null
         );
         Transaction transactionCredit = new(
            id: Guid.NewGuid(),
            date: transfer.Date,
            amount: +transfer.Amount,
            null,
            null
         );
         
         // add transactions to transfer
         transfer.Add(transactionDebit);
         transfer.Add(transactionCredit);
         
         // add transfer to debit account and beneficiary
         accountDebit.AddTransfer(transfer, beneficiary);
         
         // add transaction to debit account (Lastschrift)
         accountDebit.AddTransactions(transactionDebit, transfer, true);
         // add transaction to credit account (Gutschrift)     
         accountCredit.AddTransactions(transactionCredit, transfer, false);

         // save to transfers-/transactionsRepository and write to database
         transfersRepository.Add(transfer);
         transactionsRepository.Add(transactionDebit);
         transactionsRepository.Add(transactionCredit);
         await dataContext.SaveAllChangesAsync("Add Transfer with Transactions",ctToken);

         return new Success<Transfer>(201, transfer);
         
      } catch (Exception e) {
         return new Error<Transfer>(500, e.Message);
      }
   }

   public async Task<ResultData<Transfer>> ReverseMoneyAsync(
      Guid originalTransferId,
      Transfer reverseTransfer,
      CancellationToken ctToken = default
   ) {
      
      // check if original transfer exists
      var originalTransfer = 
         await transfersRepository.FindByIdAsync(originalTransferId, ctToken);
      if(originalTransfer == null)
         return new Error<Transfer>(404,"Reverse Money: Original transfer doesn't exist."); 
      
      // check if debit account exists (Lastschrift -> Gutschrift)
      var accountDebit = 
         await accountsRepository.FindByIdAsync(originalTransfer.AccountId,ctToken);
      if(accountDebit == null)
         return new Error<Transfer>(404,"Debit Account for the transfer doesn't exist."); 

      // check if beneficiary exists      
      if(originalTransfer.BeneficiaryId == null)
         return new Error<Transfer>(400,"Reverse Money: BeneficiaryId is null");
      var beneficiaryId = (Guid) originalTransfer.BeneficiaryId!; // convert nullable to non-nullable
      var beneficiary = 
         await beneficiariesRepository.FindByIdAsync(beneficiaryId, ctToken);
      if(beneficiary == null) 
         return new Error<Transfer>(400,"Beneficiary for the transfer doesn't exist.");
      
      // check if credit account exists (Gutschrift -> Lastschrift)
      var accountCredit = 
         await accountsRepository.FindByAsync(a => a.Iban == beneficiary.Iban, ctToken);
      if(accountCredit == null)
         return new Error<Transfer>(404,"Credit Account (Iban) for the transfer doesn't exist.");
      
      // transactionDebit + transactionCredit should not be null 
      var originalTransactions =
          await transactionsRepository.FilterByTransferIdAsync(originalTransferId, ctToken);
      if (originalTransactions.Count() != 2)
          return new Error<Transfer>(404,"Reverse Money: Original transactions are not valid.");
      // var originalTransactionDebit = originalTransactions.FirstOrDefault(t => t.Amount < 0.0);
      // if(originalTransactionDebit == null)          
      //    return new Error<Transfer>(404, "Reverse Money: Original debit transaction not found."); 
      // var originalTransactionCredit = originalTransactions.FirstOrDefault(t => t.Amount >= 0.0);
      // if(originalTransactionCredit == null) 
      //    return new Error<Transfer>(404, "Reverse Money: Original credit transaction not found.");     

      // Create two transactions
      Transaction reverseTransactionDebit = new(
         id: Guid.NewGuid(),
         date: reverseTransfer.Date,
         amount: +originalTransfer.Amount,
         null,
         null
      );
      Transaction reverseTransactionCredit = new(
         id: Guid.NewGuid(),
         date: reverseTransfer.Date,
         amount: -originalTransfer.Amount,
         null,
         null
      );
      
      // and add transfer to account
      accountDebit.AddTransfer(reverseTransfer, beneficiary);
      // Create transactionFrom (Original debit) - Lastschrift
      accountDebit.AddTransactions(reverseTransactionDebit, reverseTransfer, false);
      // Create transactionTo (Credit)  - Gutschrift     
      accountCredit.AddTransactions(reverseTransactionCredit, reverseTransfer, true);       
      
      transfersRepository.Add(reverseTransfer);             
      transactionsRepository.Add(reverseTransactionDebit);            
      transactionsRepository.Add(reverseTransactionCredit);      
      await dataContext.SaveAllChangesAsync("Add Transfer with Transactions",ctToken);     

      return new Success<Transfer>(0,reverseTransfer);
   }
}