using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class TransfersRepositoryUt: BaseRepositoryUt {

   
   [Fact]
   public async Task SelectByAccountIdAsyncUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = new List<Transfer>{ _seed.Transfer1, _seed.Transfer2 };
      
      // Act  without reference objects
      var actual = 
         await _transfersRepository.FilterByAccountIdJoinTransactionsAsync(_seed.Account1.Id);
      // Assert
      var comparison = new ComparisonBuilder()
         .IgnoreCircularReferences()
         .IgnoreProperty<Transfer>(tf => tf.Account)
         .IgnoreProperty<Transfer>(tf => tf.Beneficiary)
         .IgnoreProperty<Transfer>(tf => tf.Transactions)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));
   }

   [Fact]
   public async Task AddUt() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      var account = await _accountsRepository.FindByIdAsync(_seed.Account1.Id) ??
         throw new Exception($"Account {_seed.Account1.Id} doesn't exists."); 
      var beneficiary = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary1.Id) ??
         throw new Exception($"Beneficiary {_seed.Beneficiary1.Id} doesn't exists.");
      var transfer = _seed.Transfer1;
      // Act
      account.AddTransfer(transfer, beneficiary);
      _transfersRepository.Add(transfer);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      var actual = await _transfersRepository.FindByIdAsync(_seed.Transfer1.Id);
      // Assert
      var comparison = new ComparisonBuilder()
         .IgnoreCircularReferences()
         .IgnoreProperty<Transfer>(tf => tf.Account)
         .IgnoreProperty<Transfer>(tf => tf.Beneficiary)
         .IgnoreProperty<Transfer>(tf => tf.Transactions)
         .Create();
      Assert.True(transfer.IsDeepEqual(actual, comparison));
   }
   
}