using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using DeepEqual;
using DeepEqual.Syntax;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class OwnersRepositoryUt: BaseRepositoryUt {

   private void ShowRepository(string text){
#if DEBUG
      _dataContext.LogChangeTracker(text);
#endif
   }
   
   #region without accounts
   [Fact]
   public async Task FindByIdAsyncUt() {
      // Arrange
      var expected = _seed.Owner1;
      _ownersRepository.Add(expected);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      // Act
      var actual = await _ownersRepository.FindByIdAsync(expected.Id);
      // Assert
      Assert.NotNull(actual);
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task SelectByNameAsyncUt() {
      // Arrange
      _ownersRepository.AddRange(_seed.Owners);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      var expected = new List<OwnerDto> {
         _seed.Owner1.ToOwnerDto(),
         _seed.Owner2.ToOwnerDto()
      };
      // Act
      var actual = 
         await _ownersRepository.SelectByNameAsync("Mustermann");
      // Assert
      Assert.Equivalent(expected, actual); 
   }
   
   [Fact]
   public async Task FindByEmailAsynUt() {
      // Arrange
      _ownersRepository.AddRange(_seed.Owners);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      var expected = _seed.Owner4;
      // Act
      var actual = 
         await _ownersRepository.FindByAsync(o => o.Email!.Contains(expected.Email!));   
      // Assert
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task AddUt() {
      // Arrange
      var expected = _seed.Owner1;
      // Act
      _ownersRepository.Add(expected);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      // Assert
      var actual = await _ownersRepository.FindByIdAsync(expected.Id);
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task AddRangeUt() {
      // Arrange
      var expected = _seed.Owners;
      // Act
      _ownersRepository.AddRange(expected);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      // Assert                                 
      var actual = await _ownersRepository.SelectAsync(true);   
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task UpdateUt() {
      // Arrange
      _ownersRepository.AddRange(_seed.Owners);
      await _dataContext.SaveAllChangesAsync();
      _dataContext.ClearChangeTracker();
      // Act
      var expected = await _ownersRepository.FindByIdAsync(_seed.Owner1.Id);
      Assert.NotNull(expected);
      expected.Update("Meier Meier", "erika.meier@icloud.com");
      _ownersRepository.Update(expected);
      await _dataContext.SaveAllChangesAsync();
      // Assert
      var actual = await _ownersRepository.FindByIdAsync(_seed.Owner1.Id);
      Assert.NotNull(actual);
      Assert.Equivalent(expected, actual);
   }
   #endregion
   
   #region with accounts
   [Fact]
   public async Task FindByIdJoinAsyncUt() {
      // Arrange
      // var owner = _seed.Owner1;
      // _ownersRepository.Add(owner);
      // _dataContext.SaveAllChangesAsync("Add Owner");
      // _dataContext.ClearChangeTracker();
      // var actualOwner = await _ownersRepository.FindByIdAsync(owner.Id);
      // var account1 = _seed.Account1;
      // var account2 = _seed.Account2;
      // actualOwner.AddAccount(_seed.Account1);
      // actualOwner.AddAccount(_seed.Account2);
      //
      // _accountsRepository.Add(account1);
      // _accountsRepository.Add(account2);
      // await _dataContext.SaveAllChangesAsync("PrepareTest");
      // _dataContext.ClearChangeTracker();
      var expected = await _arrangeTest.Owner1With2AccountsAsync(_seed);
      
      // Act  with tracking
      var actual = await _ownersRepository.FindByIdJoinAsync(_seed.Owner1.Id, true);
      
      // Assert
      // Assert.Equivalent(expected, actual);
      var comparison = new ComparisonBuilder()
         .IgnoreCircularReferences()
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));
   }

   [Fact]
   public async Task FindByJoinAsyncUt() {
      // Arrange
      var expected = await _arrangeTest.Owner1With2AccountsAsync(_seed);
      // Act  with tracking
      var actual = await _ownersRepository.FindByJoinAsync(o => o.Email == _seed.Owner1.Email, true);
      // Assert
      var comparison = new ComparisonBuilder()
         .IgnoreCircularReferences()
         .Create();
      Assert.True(expected.IsDeepEqual(actual, comparison));
   }
   #endregion
}