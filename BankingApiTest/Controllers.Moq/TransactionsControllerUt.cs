using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace BankingApiTest.Controllers.Moq;

[Collection(nameof(SystemTestCollectionDefinition))]
public class TransactionsControllerUt : BaseControllerUt {
   
   
   [Fact]
   public async Task GetTransactionById_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Transaction1.Id;
      var repoResult = _seed.Transaction1;

      // mock the result of the repository
      _mockTransactionsRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.ToTransactionDto();
   
      // Act
      var actionResult = await _transactionsController.GetByIdAsync(id);
      
      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetTransactionsById_NotFound() {
      // Arrange
      _seed.Example1();
      var id = Guid.NewGuid();
      _mockTransactionsRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(null as Transaction);

      // Act
      var actionResult = await _transactionsController.GetByIdAsync(id);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }


}