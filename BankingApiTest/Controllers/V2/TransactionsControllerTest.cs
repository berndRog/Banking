using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.Dto;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class TransactionsControllerTest: BaseControllerTest {
   // [Fact]
   // public async Task FilterTest() {
   //    // Arrange
   //    await _arrangeTest.ExampleAsync(_seed);
   //    // Expected
   //    var start = new DateTime(2023, 01, 01);
   //    var end = new DateTime(2023, 07, 01);
   //    var expected = _mapper.Map<List<TransactionDto>>(
   //       _seed.Account1.Transactions.Where(t => 
   //          t.Date >= start && t.Date <= end  ));
   //    // Act
   //    var strStart = $"{start.Year}-{start.Month:d2}-{start.Day:d2}";
   //    var strEnd   = $"{end  .Year}-{end  .Month:d2}-{end  .Day:d2}";
   //    var actionResult
   //       = await _transactionsController.GetTransactionsByAccountId(_seed.Account1.Id,strStart,strEnd);
   //    // Assert
   //    THelper.IsOk(actionResult!, expected);
   // }
}