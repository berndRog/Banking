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
public class OwnersControllerUt : BaseControllerUt {

   [Fact]
   public async Task GetOwnerById_Ok() {
      // Arrange
      var id = _seed.Owner1.Id;
      var repoResult = _seed.Owner1;
      // mock the result of the repository
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.ToOwnerDto();

      // Act
      var actionResult = await _ownersController.GetByIdAsync(id);

      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetOwnerById_NotFound() {
      // Arrange
      var id = Guid.NewGuid();
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(null as Owner);

      // Act
      var actionResult = await _ownersController.GetByIdAsync(id);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task GetOwnerByName_Ok() {
      // Arrange
      var name = "Erika M";
      var repoResult = new List<Owner> { _seed.Owner1 };
      _mockOwnersRepository.Setup(r => r.SelectByNameAsync(name, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.Select(o => o.ToOwnerDto());

      // Act
      var actionResult = await _ownersController.GetByNameAsync(name);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetOwnerByName_NotFound() {
      // Arrange
      var name = "Micky M";
      _mockOwnersRepository.Setup(r => r.SelectByNameAsync(name, CancellationToken.None))
         .ReturnsAsync(new List<Owner>());

      // Act
      var actionResult = await _ownersController.GetByNameAsync(name);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }
   
   [Fact]
   public async Task CreateOwner_Created() {
      // Arrange
      var owner1Dto = _seed.Owner1.ToOwnerDto();
      // mock the repository's FindById method to return null
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(owner1Dto.Id, CancellationToken.None))
         .ReturnsAsync(null as Owner);
      // mock the repository's Add method
      _mockOwnersRepository.Setup(r => r.Add(It.IsAny<Owner>()))
         .Verifiable();
      // mock the data context's SaveAllChangesAsync method
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(null,CancellationToken.None))
         .ReturnsAsync(true);

      // Act
      var actionResult = await _ownersController.CreateAsync(owner1Dto);

      // Assert
      THelper.IsCreated(actionResult, owner1Dto);
      // Verify that the repository's Add method was called once
      _mockOwnersRepository.Verify(r => r.Add(It.IsAny<Owner>()), Times.Once);
      // Verify that the data context's SaveAllChangesAsync method was called once
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(It.IsAny<string>(),CancellationToken.None), Times.Once);
   }

   [Fact]
   public async Task CreateOwner_BadRequest() {
      // Arrange
      var owner1Dto = _seed.Owner1.ToOwnerDto();
      // mock the repository's FindByIdAsync method to return an existing owner
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(owner1Dto.Id, CancellationToken.None))
         .ReturnsAsync(_seed.Owner1);

      // Act
      var actionResult = await _ownersController.CreateAsync(owner1Dto);

      // Assert
      Assert.IsType<BadRequestObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task UpdateOwner_Updated() {
      // Arrange
      var owner1Dto = _seed.Owner1.ToOwnerDto();
      // mock the repository's FindByIdAsync method to return an existing owner
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(_seed.Owner1.Id, CancellationToken.None))
         .ReturnsAsync(_seed.Owner1);
      // mock the repository's Update method
      _mockOwnersRepository.Setup(r => r.Update(It.IsAny<Owner>()))
         .Verifiable();
      // mock the data context's SaveAllChangesAsync method
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(null, CancellationToken.None))
         .ReturnsAsync(true);

      // Act
      var actionResult = await _ownersController.UpdateAsync(owner1Dto.Id, owner1Dto);

      // Assert
      THelper.IsOk(actionResult, owner1Dto);
      // Verify that the repository's Update method was called once
      _mockOwnersRepository.Verify(r => r.Update(It.IsAny<Owner>()), Times.Once);
      // Verify that the data context's SaveAllChangesAsync method was called once
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(It.IsAny<string>(), CancellationToken.None), Times.Once);
   }

   [Fact]
   public async Task DeleteOwner_NoContent() {
      
      _seed.DoTransfer1();
      var owner = _seed.Owner1;
      var id = owner.Id;
      var account = _seed.Account1;
      
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(id, CancellationToken.None))
         .ReturnsAsync(owner);
      _mockAccountsRepository.Setup(r => r.SelectByOwnerIdAsync(id, CancellationToken.None))
         .ReturnsAsync([]);
      _mockOwnersRepository.Setup(r => r.Remove(owner))
         .Callback(() => { })
         .Verifiable();
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(null, CancellationToken.None))
         .ReturnsAsync(true);

      // Act
      var actionResult = await _ownersController.DeleteAsync(id);

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }

}