using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class AccountsRepositoryUt: BaseRepositoryUt {
   
   #region Owners <-> Accounts   
   
   [Fact]
   public async Task AddUt() {
      // Arrange
      var owner = _seed.Owner1;
      Account account = _seed.Account1;
      // Act 
      owner.AddAccount(account);        
      _accountsRepository.Add(account);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker(); // clear repository cache
      // Assert
      var actual = await _accountsRepository.FindByIdAsync(account.Id);
       
      var comparison = new ComparisonBuilder()
         //       .IgnoreCircularReferences()
         .IgnoreProperty<Account>(a => a.Owner)
         .Create();
      Assert.True(account.IsDeepEqual(actual, comparison));
   }
   
   [Fact]
   public async Task FindByIdUt() {
      // Arrange Owner1 with Account 1
      await _arrangeTest.OwnerWith1AccountAsync(_seed);  // repository cache is cleared
      // Act 
      var actual = await _accountsRepository.FindByIdAsync(_seed.Account1.Id);
      // Assert
      var comparison = new ComparisonBuilder()
         //       .IgnoreCircularReferences()
         .IgnoreProperty<Account>(a => a.Owner)
         .Create();
      Assert.True(_seed.Account1.IsDeepEqual(actual, comparison));
   }
   [Fact]
   public async Task FindByAsynUt_Iban() { 
      // Arrange
      await _arrangeTest.OwnersWithAccountsAsync(_seed); // repository cache is cleared
      // Act 
      var actual =  
         await _accountsRepository.FindByAsync(a => a.Iban.Contains("DE201000"));
      // Assert
      var comparison = new ComparisonBuilder()
         //       .IgnoreCircularReferences()
         .IgnoreProperty<Account>(a => a.Owner)
         .Create();
      Assert.True(_seed.Account3.IsDeepEqual(actual, comparison));
   }
   #endregion
   
   #region Owners <-> Accounts <-> Beneficiaries + Transfers/Transactions   
   [Fact]
   public async Task SelectByOwnerIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Owner1.Accounts;  // Account1 + Account2
      
      // Act
      // Select without join
      var actual = await _accountsRepository.SelectByOwnerIdAsync(_seed.Owner1.Id);

      // Assert
      var comparison = new ComparisonBuilder()
         //.IgnoreCircularReferences()
         .IgnoreProperty<Account>(a => a.Owner)
         .IgnoreProperty<Account>(a => a.Beneficiaries)
         .IgnoreProperty<Account>(a => a.Transfers)
         .IgnoreProperty<Account>(a => a.Transactions)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));
   }

   [Fact]
   public async Task FindByIdJoinUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Account1;
      
      // Act
      // Select with join
      var actual = await _accountsRepository.FindByIdJoinAsync(_seed.Account1.Id, true);

      // Assert
      var comparison = new ComparisonBuilder()
         .IgnoreCircularReferences()
         .IgnoreProperty<Owner>(o => o.Accounts)
         .IgnoreProperty<Account>(a => a.Beneficiaries)
         .IgnoreProperty<Account>(a => a.Transfers)
         .IgnoreProperty<Account>(a => a.Transactions)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));

   }
   #endregion

}