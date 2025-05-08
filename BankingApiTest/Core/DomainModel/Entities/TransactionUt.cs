using System;
using BankingApi.Core.DomainModel.Entities;
using Xunit;
namespace BankingApiTest.Core.DomainModel.Entities;
public class TransactionUt {

   private readonly Seed _seed = new Seed();
   
   [Fact]
   public void Ctor1Ut() {
      // Arrange
      // Act
      var actual = new Transaction();
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Transaction>(actual);
   }

   [Fact]
   public void Ctor2Ut() {
      // Arrange
      // Act
      var actual = new Transaction(
         id: _seed.Transaction1.Id,
         date: _seed.Transaction1.Date,
         amount: _seed.Transaction1.Amount,
         accountId: _seed.Account1.Id,
         transferId: _seed.Transfer1.Id
      );
      
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Transaction>(actual);
      Assert.Equal(_seed.Transaction1.Id, actual.Id);
      Assert.Equal(_seed.Transaction1.Date, actual.Date);
      Assert.Equal(_seed.Transaction1.Amount, actual.Amount);
      Assert.Equal(_seed.Account1.Id, actual.AccountId);
      Assert.Equal(_seed.Transfer1.Id, actual.TransferId);
   }
   
   [Fact]
   public void GetterUt() {
      // Arrange
      var transaction = new Transaction(
         id: _seed.Transaction1.Id,
         date: _seed.Transaction1.Date,
         amount: _seed.Transaction1.Amount,
         accountId: _seed.Account1.Id,
         transferId: _seed.Transfer1.Id
      );

      // Act
      var actualId = transaction.Id;
      var actualAmount = transaction.Amount;
      var actualAccountId = transaction.AccountId;
      var actualTransfer = transaction.Transfer;
      var actualTransferId = transaction.TransferId;
      
      // Assert
      Assert.NotNull(transaction);
      Assert.Equal(actualId, _seed.Transaction1.Id);
      Assert.Equal(actualAmount, _seed.Transaction1.Amount);
      Assert.Equal(actualAccountId, _seed.Account1.Id);
      Assert.Equal(actualTransferId, _seed.Transfer1.Id);
   }
   
   [Fact]
   public void AddDebitCreditUt(){
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      var beneficiary = _seed.Beneficiary1;
      var transfer = _seed.Transfer1;
      var transactionDebit = new Transaction();
      var transactionCredit = new Transaction();
      
      // Act
      accountDebit.AddTransfer(transfer, beneficiary);
      transactionDebit.Set(accountDebit, transfer, true);
      transactionCredit.Set(accountCredit, transfer, false);
     
      // Assert      
      Assert.Equal(transfer.Date, transactionDebit.Date);
      Assert.Equal(-transfer.Amount, transactionDebit.Amount);
      Assert.Equal(accountDebit.Id, transactionDebit.AccountId);
      Assert.Equivalent(accountDebit, transactionDebit.Account);
      Assert.Equal(transfer.Id, transactionDebit.TransferId);
      Assert.Equivalent(transfer, transactionDebit.Transfer);
      
      Assert.Equal(transfer.Date, transactionCredit.Date);
      Assert.Equal(transfer.Amount, transactionCredit.Amount);
      Assert.Equal(accountCredit.Id, transactionCredit.AccountId);
      Assert.Equivalent(accountCredit, transactionCredit.Account);
      Assert.Equal(transfer.Id, transactionCredit.TransferId);
      Assert.Equivalent(transfer, transactionCredit.Transfer);  
   }

   [Fact]
   public void ReverseDebitCreditUt(){
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      var beneficiary = _seed.Beneficiary1;
      var originalTransfer = _seed.Transfer1;
      var originalTransactionDebit = _seed.Transaction1;
      var originalTransactionCredit = _seed.Transaction2;
      accountDebit.AddTransfer(originalTransfer, beneficiary);
      originalTransactionDebit.Set(accountDebit, originalTransfer, true);
      originalTransactionCredit.Set(accountCredit, originalTransfer, false);
      
      // Act
      var reverseTransfer = new Transfer(
         id: null,
         description:"Rückbuchung",
         date: DateTime.UtcNow,
         amount: -originalTransfer.Amount
      );

      var reverseTransactionDebit = new Transaction();
      var reverseTransactionCredit = new Transaction();
      accountDebit.AddTransfer(reverseTransfer, beneficiary);
      reverseTransactionDebit.Set(accountDebit, reverseTransfer, false);
      reverseTransactionCredit.Set(accountCredit, reverseTransfer, true);
      
      // Assert  
      Assert.Equal(reverseTransfer.Date, reverseTransactionDebit.Date);
      Assert.Equal(reverseTransfer.Amount, reverseTransactionDebit.Amount);
      Assert.Equal(accountDebit.Id, reverseTransactionDebit.AccountId);
      Assert.Equivalent(accountDebit, reverseTransactionDebit.Account);
      Assert.Equal(reverseTransfer.Id, reverseTransactionDebit.TransferId);
      Assert.Equivalent(reverseTransfer, reverseTransactionDebit.Transfer);
      
      Assert.Equal(reverseTransfer.Date, reverseTransactionCredit.Date);
      Assert.Equal(-(reverseTransfer.Amount), reverseTransactionCredit.Amount);
      Assert.Equal(accountCredit.Id, reverseTransactionCredit.AccountId);
      Assert.Equivalent(accountCredit, reverseTransactionCredit.Account);
      Assert.Equal(reverseTransfer.Id, reverseTransactionCredit.TransferId);
      Assert.Equivalent(reverseTransfer,  reverseTransactionCredit.Transfer); 
   }
}