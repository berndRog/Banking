using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class TransfersControllerTest: BaseControllerTest {
   

   [Fact]
   public async Task GetByAccountIdAsyncTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Transfers
         .Where(t => t.AccountId == _seed.Account1.Id)
         .Select(t => t.ToTransferDto())
         .ToList();
      // Act
      var actionResult = await _transfersController.GetByAccountIdAsync(_seed.Account1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetByIdAsyncTest() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var expected = _seed.Transfer1.ToTransferDto();
      // Act
       var actionResult = await _transfersController.GetByIdAsync(expected.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }
   
   [Fact]
   public async Task SendMoneyTest() {
      // Arrange
      await _arrangeTest.PrepareTest1(_seed);
      
      _seed.Transfer1.SetAccount(_seed.Account1);
      _seed.Transfer1.SetBeneficiary(_seed.Beneficiary1);
      var transferDto = _seed.Transfer1.ToTransferDto();
      // Act
      var actionResult =
         await _transfersController.SendMoneyAsync(_seed.Account1.Id, transferDto);
      // Assert
      THelper.IsCreated(actionResult, transferDto);
   }

   [Fact]
   public async Task ReverseMoneyTest() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var originalTransfer = _seed.Transfer1;
      var reverseTransfer = new Transfer(
         id: Guid.NewGuid(),
         date: DateTime.UtcNow,
         description: "Reverse transfer",
         amount: -originalTransfer.Amount,
         beneficiaryId: originalTransfer.BeneficiaryId,
         accountId: originalTransfer.AccountId
      );
      var reverseTransferDto = reverseTransfer.ToTransferDto();
      
      // Act
      var actionResult = await _transfersController.ReverseMoneyAsync(
         _seed.Account1.Id,
         originalTransfer.Id,
         reverseTransferDto
      );
      
      // Assert
      THelper.IsCreated(actionResult, reverseTransferDto);
   }
   
}
