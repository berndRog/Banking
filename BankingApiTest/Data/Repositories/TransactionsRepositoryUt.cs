using System;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class TransactionsRepositoryUt: BaseRepositoryUt {
   
   
   [Fact]
   public async Task FindByIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transaction1;
      // Act 
      var actual = await _transactionsRepository.FindByIdAsync(_seed.Transaction1.Id);
      // Assert
      Assert.NotNull(actual);
      var comparison = new ComparisonBuilder()
            .IgnoreProperty<Transaction>(ta => ta.Transfer)
            .IgnoreProperty<Transaction>(ta => ta.Account)
            .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));     
   }
   
   [Fact]
   public async Task FilterByAccountIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transactions.Where(ta => ta.AccountId == _seed.Account1.Id).ToList();
      // Act 
      var actual = await _transactionsRepository.FilterByAccountIdAsync(_seed.Account1.Id, ta => true);
      // Assert
      Assert.NotNull(actual);
      var comparison = new ComparisonBuilder()
         .IgnoreProperty<Transaction>(ta => ta.Transfer)
         .IgnoreProperty<Transaction>(ta => ta.Account)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));     
   }
   
   
   [Fact]
   public async Task FilterByAccountIdAndAmountPositiveUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transactions.Where(ta =>
         ta.AccountId == _seed.Account1.Id &&
         ta.Amount > 0.0m).ToList();
      // Act 
      var actual = await _transactionsRepository.FilterByAccountIdAsync(_seed.Account1.Id, ta => ta.Amount > 0.0m);
      // Assert
      Assert.NotNull(actual);
      var comparison = new ComparisonBuilder()
         .IgnoreProperty<Transaction>(ta => ta.Transfer)
         .IgnoreProperty<Transaction>(ta => ta.Account)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));     
   }
   
   
   [Fact]
   public async Task FilterByAccountIdAndAmountNegativeUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transactions.Where(ta =>
         ta.AccountId == _seed.Account1.Id &&
         ta.Amount < 0.0m).ToList();
      // Act 
      var actual = await _transactionsRepository.FilterByAccountIdAsync(_seed.Account1.Id, ta => ta.Amount < 0.0m);
      // Assert
      Assert.NotNull(actual);
      var comparison = new ComparisonBuilder()
         .IgnoreProperty<Transaction>(ta => ta.Transfer)
         .IgnoreProperty<Transaction>(ta => ta.Account)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));     
   }

   [Fact]
   public async Task FilterListItemsByAccountIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var start = new DateTime(2023, 1, 1).ToUniversalTime();
      var end = new DateTime(2023, 6, 30).ToUniversalTime();
      
      var expected = _seed.Transactions
         .Where(ta => ta.AccountId == _seed.Account1.Id)
         .Where(ta => ta.Date >= start && ta.Date <= end)
         .Select(ta => new TransactionListItemDto(
            ta.Id,
            ta.Date,
            ta.Amount,
            ta.Transfer?.Description ?? "",
            ta.Transfer?.Beneficiary?.Name ?? "",
            ta.Transfer?.Beneficiary?.Iban ?? "",
            ta.AccountId,
            ta.TransferId ?? Guid.Empty
         )).ToList();
      
      // Act
      var actual = await _transactionsRepository.FilterListItemsByAccountIdAsync(
         _seed.Account1.Id, 
         ta => ta.Date >= start && ta.Date <= end
      );
      // Assert
      Assert.NotNull(actual);
      var comparison = new ComparisonBuilder()
         .IgnoreProperty<TransactionListItemDto>(ta => ta.TransferId)
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));
   }
}