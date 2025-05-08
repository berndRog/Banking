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
public class AccountsControllerUt : BaseControllerUt {
   [Fact]
   public async Task GetAccountsByOwnerId_Ok() {
      // Arrange
      _seed.Example1();
      var ownerId = _seed.Owner1.Id;
      var repoResult = new List<Account>() { _seed.Account1, _seed.Account2 };
      // mock the result of the repository
      _mockAccountsRepository.Setup(r => r.SelectByOwnerIdAsync(ownerId, CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.Select(a => a.ToAccountDto());

      // Act
      var actionResult = await _accountsController.GetByOwnerIdAsync(ownerId);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetAccountsByOwnerId_EmptyList() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var ownerId = _seed.Owner2.Id;
      var repoResult = new List<Account>();
      _mockAccountsRepository.Setup(r => 
         r.FilterByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.Select(a => a.ToAccountDto());

      // Act
      var actionResult = await _accountsController.GetByOwnerIdAsync(ownerId);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetAccountByIban_Ok() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var repoResult = _seed.Account1;
      var iban = _seed.Account1.Iban;
      // mock the result of the repository
      _mockAccountsRepository.Setup(r =>
         r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(repoResult);
      var expected = repoResult.ToAccountDto();

      // Act
      var actionResult = await _accountsController.GetByIbanAsync(iban);

      // Assert
      THelper.IsOk(actionResult, expected);
   }

   [Fact]
   public async Task GetAccountByIban_NotFound() {
      // Arrange
      _seed.InitAccountsForOwner1();
      var iban = _seed.Account8.Iban;
      // mock the result of the repository
      _mockAccountsRepository.Setup(r =>
            r.FindByAsync(It.IsAny<Expression<Func<Account, bool>>>(), CancellationToken.None))
         .ReturnsAsync(null as Account);

      // Act
      var actionResult = await _accountsController.GetByIbanAsync(iban);

      // Assert
      Assert.IsType<NotFoundObjectResult>(actionResult.Result);
   }

   [Fact]
   public async Task CreateAccount_Created() {
      // Arrange
      var owner = _seed.Owner1;
      var account = _seed.Account1;
      var accountDto = _seed.Account1.ToAccountDto();
      var expected = accountDto with { OwnerId = owner.Id };

      // mock the repository's methods
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(owner.Id, CancellationToken.None))
         .ReturnsAsync(owner);
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(account.Id, CancellationToken.None))
         .ReturnsAsync(null as Account);
      _mockAccountsRepository.Setup(r => r.Add(It.IsAny<Account>()))
         .Callback<Account>(a => account = a);
      _mockDataContext.Setup(c => c.SaveAllChangesAsync(null,CancellationToken.None))
         .ReturnsAsync(true);

      // Act
      var actionResult = await _accountsController.CreateAsync(owner.Id, accountDto);

      // Assert
      THelper.IsCreated(actionResult, expected);
      _mockAccountsRepository.Verify(r => r.Add(It.IsAny<Account>()), Times.Once);
      _mockDataContext.Verify(c => c.SaveAllChangesAsync(It.IsAny<string>(),CancellationToken.None), Times.Once);
   }

   [Fact]
   public async Task CreateAccount_BadRequest() {
      // Arrange
      var owner = _seed.Owner1;
      var account = _seed.Account1;
      owner.AddAccount(account);
      var accountDto = _seed.Account1.ToAccountDto();
      // mock the repository's methods
      _mockOwnersRepository.Setup(r => r.FindByIdAsync(owner.Id, CancellationToken.None))
         .ReturnsAsync(owner);
      _mockAccountsRepository.Setup(r => r.FindByIdAsync(account.Id, CancellationToken.None))
         .ReturnsAsync(account);

      // Act
      var actionResult = await _accountsController.CreateAsync(owner.Id, accountDto);

      // Assert
      Assert.IsType<BadRequestObjectResult>(actionResult.Result);
   }
}