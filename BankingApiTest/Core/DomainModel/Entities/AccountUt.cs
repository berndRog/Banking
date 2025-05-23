using System.Collections.Generic;
using BankingApi.Core.DomainModel.Entities;
using Xunit;
namespace BankingApiTest.Core.DomainModel.Entities;
public class AccountUt {
   
   private readonly Seed _seed = new();

   [Fact]
   public void Ctor(){
      // Arrange
      // Act
      var actual = new Account();
      // Assert
      Assert.NotNull(actual);      
      Assert.IsType<Account>(actual);
   }
   [Fact]
   public void Ctor2Ut(){
      // Arrange
      var owner = _seed.Owner1;
      // Act
      var actual = new Account(
         id: _seed.Account1.Id,
         iban: _seed.Account1.Iban,
         balance: _seed.Account1.Balance,
         ownerId: owner.Id
      );
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Account>(actual);
      Assert.Equal(_seed.Account1.Id, actual.Id);
      Assert.Equal(_seed.Account1.Iban, actual.Iban);
      Assert.Equal(_seed.Account1.Balance,  actual.Balance); 
      Assert.Equal(owner.Id, actual.OwnerId);
   }
   [Fact]
   public void GetterUt(){
      // Arrange
      var owner = _seed.Owner1;
      var actual = _seed.Account1;
      owner.AddAccount(actual);
      // Act
      var actualId = actual.Id;
      var actualIban = actual.Iban;
      var actualBalance = actual.Balance;
      var actualOwner = actual.Owner;
      var actualOwnerId = actual.OwnerId;
      // Assert
      Assert.Equal(actualId, _seed.Account1.Id);
      Assert.Equal(actualIban, _seed.Account1.Iban);
      Assert.Equal(actualBalance, _seed.Account1.Balance);
      Assert.Equal(actualOwner, owner);
      Assert.Equal(actualOwnerId, owner.Id);
   }
   
   #region Account -> Beneficiaries   
   [Fact]
   public void AddBeneficiariesUt(){
      // Arrange
      _seed.InitAccounts();
      var expected = new List<Beneficiary>{
         _seed.Beneficiary1, _seed.Beneficiary2
      };
      // Add
      _seed.Account1.AddBeneficiary(_seed.Beneficiary1);
      _seed.Account1.AddBeneficiary(_seed.Beneficiary2);
      // Assert
      Assert.Equal(2, _seed.Account1.Beneficiaries.Count);
      Assert.Equivalent(expected, _seed.Account1.Beneficiaries);
   }
   #endregion

   #region Accounts -> Transfers  
   [Fact]
   public void AddTransferUt() {
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var expected = new List<Transfer>{ _seed.Transfer1 };
      // Add
      _seed.Account1.AddTransfer(_seed.Transfer1, _seed.Beneficiary1);
      // Assert
      Assert.Equal(1, _seed.Account1.Transfers.Count);
      Assert.Equivalent(expected, _seed.Account1.Transfers);
   }
   #endregion


   #region Accounts -> Transaction
   [Fact]
   public void AddCreditDebitTransactionsUt(){
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      var beneficiary = _seed.Beneficiary1;
      var transfer = _seed.Transfer1;
      var transactionDebit = _seed.Transaction1;
      var transactionCredit = _seed.Transaction2;
      
      // Act
      accountDebit.AddTransfer(transfer, beneficiary);
      accountDebit.AddTransactions(transactionDebit, transfer, true);
      accountCredit.AddTransactions(transactionCredit, transfer, false);
      
      // Assert
      var actualTransfer = accountDebit.Transfers[0];
      Assert.Equal(transfer.Id, actualTransfer.Id);
      Assert.Equal(transfer.Date, actualTransfer.Date);
      Assert.Equal(transfer.Description, actualTransfer.Description);
      Assert.Equal(transfer.Amount, actualTransfer.Amount);
      Assert.Equal(transfer.AccountId, actualTransfer.AccountId);
      Assert.Equal(transfer.BeneficiaryId, actualTransfer.BeneficiaryId);
      Assert.Equal(transfer.Transactions.Count, 2);
      
      
      var actualTransactionDebit = accountDebit.Transactions[0];
      Assert.Equal(transactionDebit.Id, actualTransactionDebit.Id);
      Assert.Equal(transfer.Date,actualTransactionDebit.Date);
      Assert.Equal(-transfer.Amount, actualTransactionDebit.Amount);
      Assert.Equal(accountDebit.Id, actualTransactionDebit.AccountId);
      Assert.Equal(transfer.Id, actualTransactionDebit.TransferId);
      
      var actualTransactionCredit = accountCredit.Transactions[0];
      Assert.Equal(transactionCredit.Id, actualTransactionCredit.Id);
      Assert.Equal(transfer.Date,actualTransactionCredit.Date);
      Assert.Equal(transfer.Amount, actualTransactionCredit.Amount);
      Assert.Equal(accountCredit.Id, actualTransactionCredit.AccountId);
      Assert.Equal(transfer.Id, actualTransactionCredit.TransferId);
      
   }
   #endregion

}