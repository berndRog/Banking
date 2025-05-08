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
   /*
   [Fact]
   public void SetterUt(){
      // Arrange
      var owner = _seed.Owner1;
      var actual = new Account();
      // Act, Setter
      actual.Balance = _seed.Account1.Balance;
      actual.Owner = owner;
      actual.OwnerId = owner.Id;
      // Assert
      actual.Balance.Should().Be(_seed.Account1.Balance);
      actual.Owner.Should().Be(owner);
      actual.OwnerId.Should().Be(owner.Id);
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
      _seed.Account1.Add(_seed.Beneficiary1);
      _seed.Account1.Add(_seed.Beneficiary2);
      // Assert
      _seed.Account1.Beneficiaries.Should()
         .HaveCount(2).And
         .BeEquivalentTo(expected);
   }
   #endregion

   #region Accounts -> Transfers  
   [Fact]
   public void AddTransferUt() {
      // Arrange
      _seed.InitAccounts().InitBeneficiaries();
      var expected = new List<Transfer>{ _seed.Transfer1 };
      // Add
      _seed.Account1.Add(_seed.Transfer1, _seed.Beneficiary1);
      // Assert
      _seed.Account1.Transfers.Should()
         .HaveCount(1).And
         .BeEquivalentTo(expected);
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
      transactionDebit.Amount = -transfer.Amount;
      transactionCredit.Amount = transfer.Amount;
      // Act
      accountDebit.Add(transfer, beneficiary);
      accountDebit.Add(transactionDebit, transfer);
      accountCredit.Add(transactionCredit, transfer);
      // Assert
      accountDebit.Balance.Should().Be(1755.0);
      accountDebit.Transfers.Should().HaveCount(1);
      accountDebit.Transactions.Should().HaveCount(1);
      var actualDebit = accountDebit.Transactions[0];
      actualDebit.Date.Should().Be(transfer.Date);
      actualDebit.Amount.Should().Be(-transfer.Amount);
      actualDebit.AccountId.Should().Be(accountDebit.Id);
      actualDebit.Account?.Id.Should().Be(accountDebit.Id);
      actualDebit.TransferId.Should().Be(transfer.Id); 
      actualDebit.Transfer?.Id.Should().Be(transfer.Id);

      accountCredit.Balance.Should().Be(3845.0);
      accountCredit.Transfers.Should().HaveCount(0);
      accountCredit.Transactions.Should().HaveCount(1);
      var actualCredit = accountCredit.Transactions[0];
      actualCredit.Date.Should().Be(transfer.Date);
      actualCredit.Amount.Should().Be(transfer.Amount);
      actualCredit.AccountId.Should().Be(accountCredit.Id);
      actualCredit.Account?.Id.Should().Be(accountCredit.Id);
      actualCredit.TransferId.Should().Be(transfer.Id);
      actualCredit.Transfer?.Id.Should().Be(transfer.Id);

   }
   #endregion
   */
}