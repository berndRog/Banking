using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace BankingApiTest.Controllers.V2;

[Collection(nameof(SystemTestCollectionDefinition))]
public class OwnersControllerTest : BaseControllerTest {
   #region owners only
   public async Task GetAllAsyncTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Owners.Select(t => t.ToOwnerDto()).ToList();
      // Act
      var actionResult = await _ownersController.GetAllAsync();
      // Assert
      THelper.IsEnumerableOk(actionResult!, expected);
   }
   
   [Fact]
   public async Task GetOwnerByIdTest() {
      // Arrange
      _ownersRepository.Add(_seed.Owner1);
      await _dataContext.SaveAllChangesAsync("Add Owner1");
      _dataContext.ClearChangeTracker(); // clear repository cache
      var expected = _seed.Owner1.ToOwnerDto();
      // Act
      var actionResult = await _ownersController.GetByIdAsync(_seed.Owner1.Id);
      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetOwnerByNameTest() {
      // Arrange
      _ownersRepository.AddRange(_seed.Owners);
      await _dataContext.SaveAllChangesAsync("Add Owners");
      _dataContext.ClearChangeTracker(); // clear repository cache
      var expected = new List<OwnerDto> { _seed.Owner5.ToOwnerDto() };
      // Act
      var actionResult = await _ownersController.GetByNameAsync("Chris");
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task CreateOwnerTest() {
      // Arrange
      var owner1Dto = _seed.Owner1.ToOwnerDto();
      // Act
      var actionResult = await _ownersController.CreateAsync(owner1Dto);
      // Assert
      THelper.IsCreated(actionResult, owner1Dto);
   }

   [Fact]
   public async Task UpdateOwnerTest() {
      // Arrange
      _ownersRepository.Add(_seed.Owner1);
      await _dataContext.SaveAllChangesAsync("Add Owner1");
      _dataContext.ClearChangeTracker(); // clear repository cache
      var owner1Dto = _seed.Owner1.ToOwnerDto();
      var updOwner1Dto = owner1Dto with {
         Name = "Erika Meier",
         Email = "erika.meier@icloud.com"
      };

      // Act
      var actionResult =
         await _ownersController.UpdateAsync(owner1Dto.Id, updOwner1Dto);

      // Assert
      THelper.IsOk(actionResult, updOwner1Dto);
   }
   
   [Fact]
   public async Task DeleteTest() {
      // Arrange
      _ownersRepository.AddRange(_seed.Owners);
      await _dataContext.SaveAllChangesAsync("Add Owners");
      _dataContext.ClearChangeTracker(); // clear repository cache

      // Act
      var actionResult =
         await _ownersController.DeleteAsync(_seed.Owner1.Id);

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }
   #endregion
   
   # region owners + accounts + beneficiaries + transfers + transactions
   [Fact]
   public async Task DeleteOwner1Test() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      
      // Act
      var actionResult =
         await _ownersController.DeleteAsync(_seed.Owner1.Id);

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }
   
   [Fact]
   public async Task DeleteOwnerCascadingTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      
      // Act
      var actionResult =
         await _ownersController.DeleteAsync(_seed.Owner1.Id);

      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }
   #endregion

}


