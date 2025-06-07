using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
using BankingApi.Core.Mapping;
using BankingApiTest.Persistence.Repositories;
using Xunit;
namespace BankingApiTest.Core.UseCases;
[Collection(nameof(SystemTestCollectionDefinition))]
public class UseCasesTransferUt : BaseRepositoryUt {
  
   [Fact]
   public async Task SendMoneyAsyncUt() {
      // Arrange
      await _arrangeTest.PrepareTest1(_seed);
      var accountDebitId = _seed.Account1.Id;
      
      var date = _seed.Transfer1.Date;
      var description = _seed.Transfer1.Description;
      var amount = _seed.Transfer1.Amount;

      var accountCredit = _seed.Account6;
      
      var transferDto = new TransferDto(
         Id: _seed.Transfer1.Id,
         Date: date,
         Description: description,
         Amount: amount,
         AccountId:  _seed.Account1.Id,
         BeneficiaryId: _seed.Beneficiary1.Id
      );
      var transfer = transferDto.ToTransfer();
      
      // Act   
      var resultData = await _useCasesTransfer.SendMoneyAsync(
         accountDebitId,
         transfer
      );

      // Assert Transfer
      if (resultData is Error<Transfer>)
         Assert.Fail(resultData.Message);
      var actualTransfer = resultData.Data;
      Assert.NotNull(actualTransfer);
      Assert.Equivalent(date, actualTransfer!.Date);
      Assert.Equal(description, actualTransfer!.Description);
      Assert.Equal(amount, actualTransfer!.Amount);

      // Assert Transactions
      var transactions = await _transactionsRepository
         .FilterByAsync(ta => ta.TransferId == actualTransfer.Id)
         as IList<Transaction>;
      Assert.NotNull(transactions);
      Assert.Equal(2, transactions.Count);

      var transactionDebit = transactions.FirstOrDefault(t => t.Amount <= 0.0m);
      var transactionCredit = transactions.FirstOrDefault(t => t.Amount > 0.0m);
      Assert.NotNull(transactionDebit);
      Assert.Equal(-amount, transactionDebit!.Amount);
      Assert.Equal(date, transactionDebit!.Date);
      Assert.Equal(accountDebitId, transactionDebit!.AccountId);
      Assert.Equal(actualTransfer.Id, transactionDebit!.TransferId);
      
      Assert.NotNull(transactionCredit);
      Assert.Equal(+amount, transactionCredit!.Amount);
      Assert.Equal(date, transactionCredit!.Date);
      Assert.Equal(accountCredit.Id, transactionCredit!.AccountId);
      Assert.Equal(actualTransfer.Id, transactionDebit!.TransferId);
      
   }

   [Fact]
   public async Task ReverseMoneyAsyncUt() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var originalTransfer = _seed.Transfer1;
      var date = DateTime.UtcNow;
      var description = "Rückbuchung Erika Chris";
      var amount = -originalTransfer!.Amount;
      
      var accountCredit = _seed.Account6;
      
      var reverseTransferDto = new TransferDto(
         Id: Guid.NewGuid(),
         Date: date, 
         Description: description,
         Amount: amount,
         AccountId:  originalTransfer.AccountId,
         BeneficiaryId: (Guid) originalTransfer.BeneficiaryId!
      );
      var reverseTransfer = reverseTransferDto.ToTransfer();
      
      // Act   
      var resultData = await _useCasesTransfer.ReverseMoneyAsync(
         originalTransfer.Id,
         reverseTransfer
      );
   
      // Assert
      if (resultData is Error<Transfer>)
         Assert.Fail(resultData.Message);
      var actualTransfer = resultData.Data;
      Assert.NotNull(actualTransfer);
      Assert.Equivalent(date, actualTransfer!.Date);
      Assert.Equal(description, actualTransfer!.Description);
      Assert.Equal(amount, actualTransfer!.Amount);
      
      var reverseTransactions = await _transactionsRepository
        .FilterByAsync(ta => ta.TransferId == actualTransfer.Id)
        as IList<Transaction>;
      Assert.NotNull(reverseTransactions);
      Assert.Equal(2, reverseTransactions.Count);
      var transactionDebit = reverseTransactions.FirstOrDefault(t => t.Amount > 0.0m);   // amount > 0 Gutschrift  debit account
      var transactionCredit = reverseTransactions.FirstOrDefault(t => t.Amount <= 0.0m); // amount <= 0 Lastschrift credit account
      Assert.NotNull(transactionDebit);
      Assert.Equal(-amount, transactionDebit!.Amount);
      Assert.Equal(date, transactionDebit!.Date);
      Assert.Equal(originalTransfer.AccountId, transactionDebit!.AccountId);
      Assert.Equal(actualTransfer.Id, transactionDebit!.TransferId);

      Assert.NotNull(transactionCredit);
      Assert.Equal(+amount, transactionCredit!.Amount);
      Assert.Equal(date, transactionCredit!.Date);
      Assert.Equal(accountCredit.Id, transactionCredit!.AccountId);
      Assert.Equal(actualTransfer.Id, transactionDebit!.TransferId);

   }

   [Fact]
   public async Task SendMoneyAsyncAllTestCasesUt() {
      
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      
      // Act
      var accounts = await _accountsRepository.SelectAsync(true);
      
      // Assert with results after Testfall 11
      Assert.NotNull(accounts);
      Assert.Equal(8, accounts.Count());
      Assert.Equal(1936.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account1.Id)!.Balance);
      Assert.Equal(1675.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account2.Id)!.Balance);
      Assert.Equal(2510.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account3.Id)!.Balance);
      Assert.Equal(2322.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account4.Id)!.Balance);
      Assert.Equal(1813.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account5.Id)!.Balance);
      Assert.Equal(3845.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account6.Id)!.Balance);
      Assert.Equal(3687.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account7.Id)!.Balance);
      Assert.Equal(4612.0m, accounts.FirstOrDefault(a => a.Id == _seed.Account8.Id)!.Balance);
   }
}