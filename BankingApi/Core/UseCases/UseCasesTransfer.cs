using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core.UseCases;

public class UseCasesTransfer(
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionsRepository,
   IDataContext dataContext
) : IUseCasesTransfer {
   public async Task<ResultData<Transfer>> SendMoneyAsync(
      Guid accountDebitId, // Account from which the transfer is made (debit) 
      Transfer transfer, // Transfer data transferDto
      CancellationToken ctToken = default
   ) {
      try {
         // check if transfer with transferDto.Id already exists
         if (await transfersRepository.FindByIdAsync(transfer.Id, ctToken) != null)  
            return new Error<Transfer>(400, "SendMoney: Transfer already exists.");
         
         // check if transfer.Beneficiary is givene
         if (transfer.BeneficiaryId == null) 
            return new Error<Transfer>(404, "SendMoney: BeneficiaryId for the transfer not given.");
         
         // validate data
         var (accountDebit, accountCredit, beneficiary) = 
            await ValidateAsync(accountDebitId, transfer, ctToken);

         // Check if accountDebit, benficiary and accountCrdit exists 
         if (accountDebit == null) return new Error<Transfer>(404, "SendMoney: Debit Account for the transfer doesn't exist.");
         if (beneficiary == null) return new Error<Transfer>(404, "SendMoney: Beneficiary for the transfer doesn't exist.");
         if (accountCredit == null) return new Error<Transfer>(404, "SendMoney: Credit Account for the transfer doesn't exist.");
         
         // // check if debit account exists (Lastschrift)
         // var accountDebit = await accountsRepository.FindByIdAsync(
         //    accountDebitId, ctToken);
         // if (accountDebit == null) return new Error<Transfer>(
         //    404, "Debit Account for the transfer doesn't exist.");
         // transfer.SetAccount(accountDebit);
         //
         // // check if the benificiary exits
         // if (transfer.BeneficiaryId == null) return new Error<Transfer>(
         //    404, "BeneficiaryId for the transfer not given.");
         // var beneficiary =
         //    await beneficiariesRepository.FindByIdAsync((Guid)transfer.BeneficiaryId, ctToken);
         // if (beneficiary == null)
         //    return new Error<Transfer>(404, "Beneficiary for the transfer doesn't exist.");
         //
         // // check if credit account exists (Gutschrift)
         // var accountCredit = await accountsRepository.FindByAsync(
         //    a => a.Iban == beneficiary.Iban, ctToken);
         // if (accountCredit == null) return new Error<Transfer>(
         //    404, "Credit Account (Iban) for the transfer doesn't exist.");
         //
         //
         // Transaction transactionDebit = new(
         //    id: Guid.NewGuid(),
         //    date: transfer.Date,
         //    amount: -transfer.Amount
         // );
         // Transaction transactionCredit = new(
         //    id: Guid.NewGuid(),
         //    date: transfer.Date,
         //    amount: +transfer.Amount
         // );
         
         // Setup transfer and create transactions
         transfer.SetAccount(accountDebit);
         var (transactionDebit, transactionCredit) = CreateTransactionsFromTransfer(transfer);
         
         // add transfer to debit account and beneficiary
         accountDebit.AddTransfer(transfer, beneficiary);

         // add transaction to transfer and debit account (Lastschrift)
         accountDebit.AddTransactions(transactionDebit, transfer, true);
         // add transaction to transfer and credit account (Gutschrift)     
         accountCredit.AddTransactions(transactionCredit, transfer, false);

         // save to Transfers- and TransactionsRepository and write to database
         transfersRepository.Add(transfer);
         await dataContext.SaveAllChangesAsync("Add Transfer with Transactions", ctToken);

         return new Success<Transfer>(201, transfer);
      }
      catch (Exception e) {
         return new Error<Transfer>(500, e.Message);
      }
   }

   private async Task<(Account,Account,Beneficiary)> ValidateAsync(
      Guid accountId,
      Transfer transfer,
      CancellationToken ctToken
   ) {
      // check if debit account exists (Lastschrift)
      var accountDebit = await accountsRepository.FindByIdAsync(
         accountId, ctToken);
      
      // check if the benificiary exits
      var beneficiary =
         await beneficiariesRepository.FindByIdAsync((Guid)transfer.BeneficiaryId, ctToken);
  
      // check if credit account exists (Gutschrift)
      var accountCredit = await accountsRepository.FindByAsync(
         a => a.Iban == beneficiary.Iban, ctToken);

      return (accountDebit, accountCredit, beneficiary);
   }
   
   private (Transaction, Transaction) CreateTransactionsFromTransfer(Transfer transfer) {
      Transaction transactionDebit = new(
         id: Guid.NewGuid(),
         date: transfer.Date,
         amount: -transfer.Amount
      );
      Transaction transactionCredit = new(
         id: Guid.NewGuid(),
         date: transfer.Date,
         amount: +transfer.Amount
      );
      return (transactionDebit, transactionCredit);
   }


   public async Task<ResultData<Transfer>> ReverseMoneyAsync(
      Guid originalTransferId,
      Transfer reverseTransfer,
      CancellationToken ctToken = default
   ) {
      try {
         // check if original transfer exists
         var transfer =
            await transfersRepository.FindByIdAsync(originalTransferId, ctToken);
         if (transfer == null) return new Error<Transfer>(404, "ReverseMoney: Original transfer doesn't exist.");
         
         // check if transfer.BeneficaryId exists
         if (transfer.BeneficiaryId == null) return new Error<Transfer>(404, "ReverseMoney: BeneficiaryId for the transfer not given.");
         
         // validate data
         var (accountDebit, accountCredit, beneficiary) = 
            await ValidateAsync(transfer.AccountId, transfer, ctToken);

         // Check if accountDebit, benficiary and accountCrdit exists 
         if (accountDebit == null) return new Error<Transfer>(404, "ReversMoney: Debit Account for the transfer doesn't exist.");
         if (beneficiary == null) return new Error<Transfer>(404, "ReverseMoney: Beneficiary for the transfer doesn't exist.");
         if (accountCredit == null) return new Error<Transfer>(404, "ReverseMoney: Credit Account for the transfer doesn't exist.");
         
         // Setup transfer and create transactions
         reverseTransfer.SetAccount(accountDebit);
         var (reverseTransactionDebit, reverseTransactionCredit) = CreateTransactionsFromTransfer(reverseTransfer);
         
         // add transfer to debit account
         accountDebit.AddTransfer(reverseTransfer, beneficiary);
         
         // add transactionDebit to debit account (Gutschrift)
         accountDebit.AddTransactions(reverseTransactionDebit, reverseTransfer, true);    // use isDebit = true
         // add transactionCredit to credit account (Lastschrift)     
         accountCredit.AddTransactions(reverseTransactionCredit, reverseTransfer, false); // use isDebit = false
         
         // save to transfers-/transactionsRepository and write to database
         transfersRepository.Add(reverseTransfer);
         //transactionsRepository.Add(reverseTransactionDebit);
         //transactionsRepository.Add(reverseTransactionCredit);
         await dataContext.SaveAllChangesAsync("Add Transfer with Transactions", ctToken);

         return new Success<Transfer>(0, reverseTransfer);
      }
      catch (Exception e) {
         return new Error<Transfer>(500, e.Message);
      }
   }
   
   
   
}