using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
namespace BankingApiTest.Controllers.Moq;

[Collection(nameof(SystemTestCollectionDefinition))]
public class TransfersControllerUt : BaseControllerUt {
   
   [Fact]
   public async Task GetTransfersByAcccountId_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Account1.Id;
      var repoResult = 
         new List<Transfer> { _seed.Transfer1, _seed.Transfer2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(_seed.Account1);
      _mockTransfersRepository.Setup(r => r.SelectByAccountIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      
      var expected = repoResult.Select(t => t.ToTransferDto());
      
      // Act
      var actionResult = await _transfersController.GetByAccountIdAsync(id);

      // Assert
      THelper.IsOk(actionResult, expected);
   }
   
   [Fact]
   public async Task GetTransferById_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Transfer1.Id;
      var repoResult = _seed.Transfer1;
      // mock the result of the repository
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.ToTransferDto();
   
      // Act
      var actionResult = await _transfersController.GetByIdAsync(id);
      
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetTransferById_NotFound() {
      // Arrange
      _seed.Example1();
      var id = Guid.NewGuid();
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(null as Transfer);

      // Act
      var actionResult = await _transfersController.GetByIdAsync(id);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task SendMoney_Ok() {
      // Arrange
      _seed.PrepareTest1();
//      _seed.Transfer1.AccountId = _seed.Account1.Id;
//      _seed.Transfer1.BeneficiaryId = _seed.Beneficiary1.Id;
      _seed.Transfer1.SetAccount(_seed.Account1);
      _seed.Transfer1.SetBeneficiary(_seed.Beneficiary1);
      var expected = _seed.Transfer1.ToTransferDto();
      
      var id = _seed.Transfer1.Id;
      var accountDebit = _seed.Account1;
      var beneficiary = _seed.Beneficiary1;
      var accountCredit = _seed.Account6;
      
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
         .ReturnsAsync(accountDebit);
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
         .ReturnsAsync(beneficiary);
      _mockAccountsRepository.Setup(r => r.FindByAsync(a => a.Iban == beneficiary.Iban, CancellationToken.None))
         .ReturnsAsync(accountCredit);
      
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
         .ReturnsAsync(null as Transfer);
      _mockTransfersRepository.Setup(r => r.Add(It.IsAny<Transfer>()))
         .Verifiable();
      _mockTransactionsRepository.Setup(r => r.Add(It.IsAny<Transaction>()))
         .Verifiable();
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(It.IsAny<string>(), CancellationToken.None))
         .ReturnsAsync(true);
      
      // Act
      var actionResult = await _transfersController.SendMoneyAsync(accountDebit.Id, expected);

      // Assert
      THelper.IsCreated(actionResult, expected);
   }

   [Fact]
   public async Task ReverseMoney_Ok() {
      // Arrange
      _seed.DoTransfer1();
      _seed.Transfer1.SetAccount(_seed.Account1);
      _seed.Transfer1.SetBeneficiary(_seed.Beneficiary1);
      //_seed.Transfer1.AccountId = _seed.Account1.Id;
      //_seed.Transfer1.BeneficiaryId = _seed.Beneficiary1.Id;
      
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
      
      var id = _seed.Transfer1.Id;
      var accountDebit = _seed.Account1;
      var beneficiary = _seed.Beneficiary1;
      var accountCredit = _seed.Account6;
      
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
         .ReturnsAsync(accountDebit);
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
         .ReturnsAsync(beneficiary);
      _mockAccountsRepository.Setup(r => r.FindByAsync(a => a.Iban == beneficiary.Iban, CancellationToken.None))
         .ReturnsAsync(accountCredit);

      _mockTransfersRepository.Setup(r => r.FindByIdAsync(originalTransfer.Id, CancellationToken.None))
         .ReturnsAsync(originalTransfer);
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(reverseTransfer.Id, CancellationToken.None))
         .ReturnsAsync(null as Transfer);
      _mockTransfersRepository.Setup(r => r.Add(It.IsAny<Transfer>()))
         .Verifiable();
      _mockTransactionsRepository.Setup(r => r.Add(It.IsAny<Transaction>()))
         .Verifiable();
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(It.IsAny<string>(), CancellationToken.None))
         .ReturnsAsync(true);
      
      // Act
      var actionResult = await _transfersController.ReverseMoneyAsync(
         _seed.Account1.Id,
         originalTransfer.Id, 
         reverseTransferDto
      );

      // Assert
      THelper.IsCreated(actionResult, reverseTransferDto);
      _mockDataContext.Verify(c => 
         c.SaveAllChangesAsync(It.IsAny<string>(),CancellationToken.None), Times.Once);
   }

   /*
   // [Fact]
   // public async Task DeleteOwner_NoContent() {
   //    var owner = _seed.Owner1;
   //    var id = owner.Id;
   //
   //    _mockOwnersRepository.Setup(r => r.FindByIdAsync(id))
   //       .ReturnsAsync(owner);
   //    _mockOwnersRepository.Setup(r => r.Remove(owner))
   //       .Callback<Owner>(ownerToRemove => { ownerToRemove = owner; });
   //    _mockDataContext.Setup(c => c.SaveAllChangesAsync())
   //       .ReturnsAsync(true);
   //
   //    // Act
   //    var result = await _ownersController.DeleteOwner(id);
   //
   //    // Assert
   //    _mockOwnersRepository.Verify(r => r.Remove(owner), Times.Once);
   //    _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   // }
   */
}