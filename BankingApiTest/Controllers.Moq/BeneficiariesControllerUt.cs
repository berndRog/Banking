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
public class BeneficiariesControllerUt : BaseControllerUt {
   
   [Fact]
   public async Task GetBeneficiariesByAcccountId_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Account1.Id;
      var repoResult = new List<Beneficiary> { _seed.Beneficiary1, _seed.Beneficiary2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(_seed.Account1);
      _mockBeneficiariesRepository.Setup(r => r.SelectByAccountIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.Select(b => b.ToBeneficiaryDto());

      // Act
      var actionResult = await _beneficiariesController.GetByAccountIdAsync(id);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
   [Fact]
   public async Task GetBeneficiaryById_Ok() {
      // Arrange
      _seed.Example1();
      var id = _seed.Beneficiary1.Id;
      var repoResult = _seed.Beneficiary1;
      // mock the result of the repository
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.ToBeneficiaryDto();

      // Act
      var actionResult = await _beneficiariesController.GetByIdAsync(id);
      
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetBeneficiaryById_NotFound() {
      // Arrange
      _seed.Example1();
      var id = Guid.NewGuid();
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(null as Beneficiary);

      // Act
      var actionResult = await _beneficiariesController.GetByIdAsync(id);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task GetBeneficiariesByName_Ok() {
      // Arrange
      _seed.Example1();
      var name = "Christ";
      var repoResult = new List<Beneficiary> { _seed.Beneficiary1, _seed.Beneficiary2, _seed.Beneficiary3 };
      _mockBeneficiariesRepository.Setup(r => r.SelectByNameAsync(name, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.Select(b => b.ToBeneficiaryDto());

      // Act
      var actionResult = await _beneficiariesController.GetByNameAsync(name);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
   [Fact]
   public async Task CreateBeneficiaryUt_Created() {
      // Arrange
      // credit account
      var iban = _seed.Beneficiary1.Iban; // account 6
      // debit account
      _seed.Beneficiary1.SetAccount(_seed.Account1);
      var beneficiary1Dto = _seed.Beneficiary1.ToBeneficiaryDto();
      
      // mock the repositories 
      // beneficiary does not exist
      _mockBeneficiariesRepository.Setup(r => 
            r.FindByIdAsync(_seed.Beneficiary1.Id, CancellationToken.None))
         .ReturnsAsync(null as Beneficiary);
      // credit account
      _mockAccountsRepository.Setup(r => 
            r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(_seed.Account6);
      // debit account   
      _mockAccountsRepository.Setup(r => 
            r.FindByIdAsync(_seed.Account1.Id, CancellationToken.None))
         .ReturnsAsync(_seed.Account1);

      _mockBeneficiariesRepository.Setup(r => 
            r.Add(It.IsAny<Beneficiary>()))
         .Verifiable();
      _mockDataContext.Setup(c => 
            c.SaveAllChangesAsync(It.IsAny<string>(), CancellationToken.None))
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.CreateAsync(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      THelper.IsCreated(actionResult, beneficiary1Dto);
      _mockBeneficiariesRepository.Verify(r => 
         r.Add(It.IsAny<Beneficiary>()), Times.Once);
      _mockDataContext.Verify(c => 
         c.SaveAllChangesAsync(It.IsAny<string>(), CancellationToken.None), Times.Once);
   }
   
   [Fact]
   public async Task CreateBeneficiaryUt_BadRequest() {
      // Arrange
      // debit account

      // credit account
      _seed.Beneficiary1.SetAccount(_seed.Account1);
      var beneficiary1Dto = _seed.Beneficiary1.ToBeneficiaryDto();
      
      // mock the repositories 
      _mockBeneficiariesRepository.Setup(r => 
            r.FindByIdAsync(_seed.Beneficiary1.Id, CancellationToken.None))
         .ReturnsAsync(_seed.Beneficiary1);
      
      // Act
      var actionResult = await _beneficiariesController.CreateAsync(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      Assert.IsType<BadRequestObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task CreateBeneficiaryUt_CreditNotFound() {
      // Arrange
      // debit account

      // credit account
      _seed.Beneficiary1.SetAccount(_seed.Account1);
      var beneficiary1Dto = _seed.Beneficiary1.ToBeneficiaryDto();
      
      // mock the repositories 
      _mockBeneficiariesRepository.Setup(r => 
            r.FindByIdAsync(_seed.Beneficiary1.Id, CancellationToken.None))
         .ReturnsAsync(null as Beneficiary);
      // credit account
      _mockAccountsRepository.Setup(r => 
            r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(null as Account);
      
      // Act
      var actionResult = await _beneficiariesController.CreateAsync(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }
   
   [Fact]
   public async Task CreateBeneficiaryUt_DebitNotFound() {
      // Arrange
      // debit account

      // credit account
      _seed.Beneficiary1.SetAccount(_seed.Account1);
      var beneficiary1Dto = _seed.Beneficiary1.ToBeneficiaryDto();
      
      // mock the repositories 
      _mockBeneficiariesRepository.Setup(r => 
            r.FindByIdAsync(_seed.Beneficiary1.Id, CancellationToken.None))
         .ReturnsAsync(null as Beneficiary);
      // credit account
      _mockAccountsRepository.Setup(r => 
            r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(_seed.Account6);
      // debit account   
      _mockAccountsRepository.Setup(r => 
            r.FindByIdAsync(_seed.Account1.Id, CancellationToken.None))
         .ReturnsAsync(null as Account);
      
      // Act
      var actionResult = await _beneficiariesController.CreateAsync(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }

   /*
   [Fact]
   public async Task DeleteBeneficiary_NoContent() {
      // Arrange
      _seed.Example1();
      var id = _seed.Beneficiary1.Id;
      var repoResult = new List<Transfer> { _seed.Transfer1 };
      
      // mock the repositories
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockTransfersRepository.Setup(r => r.SelectByBeneficiaryIdAsync(id))
         .ReturnsAsync(repoResult);
      
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(_seed.Beneficiary1);
      _mockBeneficiariesRepository.Setup(r => r.Remove(It.IsAny<Beneficiary>()))
         .Verifiable();
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.DeleteBeneficiary(
         _seed.Account1.Id, _seed.Beneficiary1.Id);
      
      // Assert
      THelper.IsNoContent(actionResult!);
      _mockOwnersRepository.Verify();
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }
*/
}