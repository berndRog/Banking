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

   /*
   [Fact]
   public async Task CreateBeneficiary_Created() {
      // Arrange
      // debit account
      var lastname = _seed.Beneficiary1.Lastname; // owner 5
      var iban = _seed.Beneficiary1.Iban; // account 6
      // credit account
      _seed.Beneficiary1.AccountId = _seed.Account1.Id;
      var beneficiary1Dto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      Beneficiary? addedBeneficiary = null;
      
      // mock the repositories 
      _mockOwnersRepository.Setup(r => r.LikeLastnameByAsync(lastname))
         .ReturnsAsync(new List<Owner> { _seed.Owner5 });
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(_seed.Account1);
      // beneficiary does not exist
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(null as Beneficiary);
      _mockBeneficiariesRepository.Setup(r => r.Add(It.IsAny<Beneficiary>()))
         .Callback<Beneficiary>(beneficiary => addedBeneficiary = beneficiary);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.CreateBeneficiary(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      THelper.IsCreated(actionResult, beneficiary1Dto);
      _mockBeneficiariesRepository.Verify(r => r.Add(It.IsAny<Beneficiary>()), Times.Once);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Once);
   }

   [Fact]
   public async Task CreateBeneficiary_Conflict() {
      // Arrange
      // debit account
      var firstname = _seed.Beneficiary1.Firstname; // owner 5
      var lastname = _seed.Beneficiary1.Lastname; // owner 5
      var iban = _seed.Beneficiary1.Iban; // account 6
      // credit account
      _seed.Beneficiary1.AccountId = _seed.Account1.Id;
      var beneficiary1Dto = _mapper.Map<BeneficiaryDto>(_seed.Beneficiary1);
      Beneficiary? addedBeneficiary = null;
      
      // mock the repositories 
      _mockOwnersRepository.Setup(r => r.LikeLastnameByAsync(lastname))
         .ReturnsAsync(new List<Owner> { _seed.Owner5 });
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(_seed.Account1.Id))
         .ReturnsAsync(_seed.Account1);
      _mockAccountsRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>()))
         .ReturnsAsync(_seed.Account1);
      // beneficiary already exists
      _mockBeneficiariesRepository.Setup(r => r.FindByIdAsync(_seed.Beneficiary1.Id))
         .ReturnsAsync(_seed.Beneficiary1);
      _mockBeneficiariesRepository.Setup(r => r.Add(It.IsAny<Beneficiary>()))
         .Callback<Beneficiary>(beneficiary => addedBeneficiary = beneficiary);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync())
         .ReturnsAsync(true);

      // Act
      var actionResult = await _beneficiariesController.CreateBeneficiary(_seed.Account1.Id, beneficiary1Dto);

      // Assert
      THelper.IsConflict(actionResult);
      _mockOwnersRepository.Verify(r => r.Add(It.IsAny<Owner>()), Times.Never);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(), Times.Never);
   }

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