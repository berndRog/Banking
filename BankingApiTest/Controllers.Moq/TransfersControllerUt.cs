using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using Moq;
using Xunit;
namespace BankingApiTest.Controllers.Moq;

[Collection(nameof(SystemTestCollectionDefinition))]
public class TransfersControllerUt : BaseControllerUt {
  
  /* 
   [Fact]
   public async Task GetTransfersByAcccountId_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Account1.Id;
      var repoResult = 
         new List<Transfer> { _seed.Transfer1, _seed.Transfer2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(_seed.Account1);
      _mockTransfersRepository.Setup(r => r.SelectByAccountIdAsync(id))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<IEnumerable<TransferDto>>(repoResult);
      
      // Act
      var actionResult = await _transfersController.GetTransfersByAccountId(id);

      // Assert
      THelper.IsOk(actionResult!, expected);
      
   }
   
   [Fact]
   public async Task GetTransferById_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Transfer1.Id;
      var repoResult = _seed.Transfer1;
      // mock the result of the repository
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(repoResult);
      var expected = _mapper.Map<TransferDto>(repoResult);
   
      // Act
      var actionResult = await _transfersController.GetTransferById(id);
      
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task GetTransferById_NotFound() {
      // Arrange
      _seed.Example1();
      var id = Guid.NewGuid();
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(id))
         .ReturnsAsync(null as Transfer);

      // Act
      var actionResult = await _transfersController.GetTransferById(id);

      // Assert
      THelper.IsNotFound(actionResult);
   }

   [Fact]
   public async Task SendMoney_Ok() {
      // Arrange
      _seed.PrepareTest1();
      _seed.Transfer1.AccountId = _seed.Account1.Id;
      _seed.Transfer1.BeneficiaryId = _seed.Beneficiary1.Id;
      var transferDto = _mapper.Map<TransferDto>(_seed.Transfer1);
      
      var id = _seed.Transfer1.Id;
      var accountDebit = _seed.Account1;
      var beneficiary = _seed.Beneficiary1;
      var accountCredit = _seed.Account6;
      
      Transfer? addedTransfer = null;
      Transaction? addedTransaction = null;
      
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
         .ReturnsAsync(accountDebit);
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
         .ReturnsAsync(beneficiary);
      _mockAccountsRepository.Setup(r => r.FindByAsync(a => a.Iban == beneficiary.Iban))
         .ReturnsAsync(accountCredit);
      
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
         .ReturnsAsync(null as Transfer);
      _mockTransfersRepository.Setup(r => r.Add(It.IsAny<Transfer>()))
         .Callback<Transfer>(transfer => addedTransfer = transfer);
      _mockTransactionsRepository.Setup(r => r.Add(It.IsAny<Transaction>()))
         .Callback<Transaction>(transaction => addedTransaction = transaction);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);
      
      var expected = _mapper.Map<TransferDto>(transferDto);

      // Act
      var actionResult = await _transfersController.SendMoney(accountDebit.Id, transferDto);

      // Assert
      THelper.IsCreated(actionResult, expected);
      _mockTransfersRepository.Verify(r => r.Add(It.IsAny<Transfer>()), Times.Once);
      _mockTransactionsRepository.Verify(r => r.Add(It.IsAny<Transaction>()), Times.Exactly(2));
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }
   
   [Fact]
   public async Task ReverseMoney_Ok() {
      // Arrange
      _seed.DoTransfer1();
      _seed.Transfer1.AccountId = _seed.Account1.Id;
      _seed.Transfer1.BeneficiaryId = _seed.Beneficiary1.Id;
      
      var originalTransfer = _seed.Transfer1;
      var reverseTransfer = new Transfer {
         Id = Guid.NewGuid(),
         Date = DateTime.UtcNow,
         Description = "Reverse transfer",
         Amount = -originalTransfer.Amount,
         BeneficiaryId = originalTransfer.BeneficiaryId,
         AccountId = originalTransfer.AccountId
      };
      var reverseTransferDto = _mapper.Map<TransferDto>(reverseTransfer);
      
      var id = _seed.Transfer1.Id;
      var accountDebit = _seed.Account1;
      var beneficiary = _seed.Beneficiary1;
      var accountCredit = _seed.Account6;
      
      Transfer? addedTransfer = null;
      Transaction? addedTransaction = null;
      
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
         .ReturnsAsync(accountDebit);
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(It.IsAny<Guid>()))
         .ReturnsAsync(beneficiary);
      _mockAccountsRepository.Setup(r => r.FindByAsync(a => a.Iban == beneficiary.Iban))
         .ReturnsAsync(accountCredit);

      _mockTransfersRepository.Setup(r => r.FindByIdAsync(originalTransfer.Id))
         .ReturnsAsync(originalTransfer);
      _mockTransfersRepository.Setup(r => r.FindByIdAsync(reverseTransfer.Id))
         .ReturnsAsync(null as Transfer);
      _mockTransfersRepository.Setup(r => r.Add(It.IsAny<Transfer>()))
         .Callback<Transfer>(transfer => addedTransfer = transfer);
      _mockTransactionsRepository.Setup(r => r.Add(It.IsAny<Transaction>()))
         .Callback<Transaction>(transaction => addedTransaction = transaction);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);
    

      // Act
      var actionResult = await _transfersController.ReverseMoney(
         _seed.Account1.Id,
         originalTransfer.Id, 
         reverseTransferDto
      );

      // Assert
      THelper.IsCreated(actionResult, reverseTransferDto);
      _mockTransfersRepository.Verify(r => r.Add(It.IsAny<Transfer>()), Times.Once);
      _mockTransactionsRepository.Verify(r => r.Add(It.IsAny<Transaction>()), Times.Exactly(2));
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }

   
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