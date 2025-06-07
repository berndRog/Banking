using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Mapping;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class TransfersControllerTest: BaseControllerTest {
   

   [Fact]
   public async Task SelectAsyncTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transactions.Select(t => t.ToTransactionDto()).ToList();
      // Act
      var actionResult = await _transactionsController.GetAllAsync();
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
   [Fact]
   public async Task GetByAccountIdAsyncTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transactions
         .Where(t => t.AccountId == _seed.Account1.Id)
         .Select(t => t.ToTransactionDto())
         .ToList();
      // Act
      var actionResult = await _transactionsController.GetByAccountIdAsync(_seed.Account1.Id,"2023-01-01","2023-12-31");
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetByIdAsyncTest() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var transactions = await _transactionsRepository.SelectAsync(true);
      var expected = transactions.First().ToTransactionDto();
      
      // Act
      var actionResult = await _transactionsController.GetByIdAsync(expected.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }
   
   
}
