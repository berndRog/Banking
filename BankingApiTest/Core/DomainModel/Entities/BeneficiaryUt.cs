using BankingApi.Core.DomainModel.Entities;
using Xunit;
namespace BankingApiTest.Core.DomainModel.Entities;
public class BeneficiaryUt {

   private readonly Seed _seed = new Seed();
   
   [Fact]
   public void CtorUt() {
      // Arrange
      // Act
      var actual = new Beneficiary();
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Beneficiary>(actual);
   }
   [Fact]
   public void Ctor2Ut() {
      // Arrange
      // Act
      var actual = new Beneficiary(
         id: _seed.Beneficiary1.Id,
         name: _seed.Beneficiary1.Name,
         iban: _seed.Beneficiary1.Iban,
         accountId: _seed.Account1.Id
      );
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Beneficiary>(actual);
      Assert.Equal(_seed.Beneficiary1.Id, actual.Id);
      Assert.Equal(_seed.Beneficiary1.Name,  actual.Name); 
      Assert.Equal(_seed.Beneficiary1.Iban, actual.Iban);
      Assert.Equal(_seed.Account1.Id, actual.AccountId);
   }
   [Fact]
   public void SetterGetterUt() {
      // Arrange
      // Act
      var actual = new Beneficiary(
         id: _seed.Beneficiary1.Id,
         name: _seed.Beneficiary1.Name,
         iban: _seed.Beneficiary1.Iban,
         accountId: _seed.Account1.Id
      );
      var _id = actual.Id;
      var _firstname = actual.Name;
      var _iban = actual.Iban;
      var _accountId = actual.AccountId;
      // Assert
      Assert.Equal(_id, _seed.Beneficiary1.Id);
      Assert.Equal(_firstname, _seed.Beneficiary1.Name);
      Assert.Equal(_iban, _seed.Beneficiary1.Iban);
   }
   
   [Fact]
   public void SetAccountUt() {
      // Arrange
      var actual = _seed.Beneficiary1;
      var account = _seed.Account1;
      // Act
      actual.SetAccount(account);
      // Assert
      Assert.Equal(_seed.Account1.Id, actual.AccountId);
   }
}