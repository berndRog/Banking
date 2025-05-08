using System;
using BankingApi.Core.DomainModel.Entities;
using Xunit;
namespace BankingApiTest.Core.DomainModel.Entities;
public class TransferUt {

   private readonly Seed _seed = new Seed();
   
   [Fact]
   public void Ctor1Ut() {
      // Arrange
      // Act
      var actual = new Transfer();
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Transfer>(actual);
   }

   [Fact]
   public void Ctor2Ut() {
      // Arrange
      // Act
      var actual = new Transfer(
         id: _seed.Transfer1.Id,
         description: _seed.Transfer1.Description,
         date: _seed.Transfer1.Date,
         amount: _seed.Transfer1.Amount,
         accountId: _seed.Account1.Id,
         beneficiaryId: _seed.Beneficiary1.Id
      );
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Transfer>(actual);
      Assert.Equal(_seed.Transfer1.Id, actual.Id);
      Assert.Equal(_seed.Transfer1.Description, actual.Description);
      Assert.Equal(_seed.Transfer1.Date, actual.Date);
      Assert.Equal(_seed.Transfer1.Amount,  actual.Amount);    
      Assert.Equal(_seed.Account1.Id, actual.AccountId);
      Assert.Equal(_seed.Beneficiary1.Id, actual.BeneficiaryId);
   }

   [Fact]
   public void GetterUt() {
      // Arrange
      var transfer = new Transfer(
         id: _seed.Transfer1.Id,
         description: _seed.Transfer1.Description,
         date: _seed.Transfer1.Date,
         amount: _seed.Transfer1.Amount,
         accountId: _seed.Account1.Id,
         beneficiaryId: _seed.Beneficiary1.Id
      );
      // Act
      var actualId = transfer.Id;
      var actualDescription = transfer.Description;
      var actualDate = transfer.Date;
      var actualAmount = transfer.Amount;
      var actualAccount = transfer.Account;
      var actualAccountId = transfer.AccountId;
      var actualBeneficiary = transfer.Beneficiary;
      var actualBeneficiaryId = transfer.BeneficiaryId;
      // Assert
      Assert.Equal(actualId, _seed.Transfer1.Id);
      Assert.Equal(actualDescription, _seed.Transfer1.Description);
      Assert.Equal(actualDate, _seed.Transfer1.Date);
      Assert.Equal(actualAmount, _seed.Transfer1.Amount);
      Assert.Equal(actualAccountId, _seed.Account1.Id);
      Assert.Equal(actualBeneficiaryId, _seed.Beneficiary1.Id);
   }
   
   [Fact]
   public void SetAccountUt() {
      // Arrange
      var transfer = new Transfer();
      var account = _seed.Account1;
      // Act
      transfer.SetAccount(account);
      // Assert
      Assert.Equivalent(_seed.Account1, transfer.Account);
      Assert.Equal(_seed.Account1.Id, transfer.AccountId);
   }

   [Fact]
   public void SetBeneficiaryUt() {
      // Arrange
      var transfer = new Transfer();
      var beneficiary = _seed.Beneficiary1;
      // Act
      transfer.SetBeneficiary(beneficiary);
      // Assert
      Assert.Equivalent(_seed.Beneficiary1, transfer.Beneficiary);
      Assert.Equal(_seed.Beneficiary1.Id, transfer.BeneficiaryId);
   }
   
}