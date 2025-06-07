using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.Dtos;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class AccountsControllerTest: BaseControllerTest {

   #region Accounts<->Owners
   [Fact]
   public async Task GetAccountsAsyncTest() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAsync(_seed);
      var expected = _seed.Accounts.Select(t => t.ToAccountDto()).ToList();
      // Act
      var actionResult = await _accountsController.GetAllAsync();
      // Assert
      THelper.IsEnumerableOk(actionResult!, expected);
   }
   
   [Fact]
   public async Task GetAccountsByOwnerIdTest() {
      // Arrange
      await _arrangeTest.Owner1With2AccountsAsync(_seed);
      var expected = new List<AccountDto> {
         _seed.Account1.ToAccountDto(),
         _seed.Account2.ToAccountDto(),
      };
      // Act
      var actionResult = await _accountsController.GetByOwnerIdAsync(_seed.Owner1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }
   [Fact]
   public async Task GetAccountByIdTest() {
      // Arrange
      await _arrangeTest.Owner1With2AccountsAsync(_seed);
      var expected = _seed.Account1.ToAccountDto();
      // Act
      var actionResult = await _accountsController.GetByIdAsync(_seed.Account1.Id);
      // Assert
      THelper.IsOk(actionResult!, expected);
   }

   [Fact]
   public async Task FindByIbanTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Account6.ToAccountDto();
      // Act
      var actionResult = await _accountsController.GetByIbanAsync("DE50 1000 0000 0000 0000 00");
      // Assert
      THelper.IsOk(actionResult, expected);
   }
   
   [Fact]
   public async Task PostTest() {
      // Arrange
      _ownersRepository.Add(_seed.Owner1);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();   
      _seed.Owner1.AddAccount(_seed.Account1);
      var expected = _seed.Account1.ToAccountDto();
      // Act
      var actionResult =
         await _accountsController.CreateAsync(_seed.Owner1.Id, expected);
      // Assert
      THelper.IsCreated(actionResult, expected);
   }
   #endregion
   
   #region Accounts with all references
   [Fact]
   public async Task DeleteCascadingTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      // Act
      var actionResult =
         await _accountsController.DeleteAsync(_seed.Owner1.Id,_seed.Account1.Id);
      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }
   #endregion
}