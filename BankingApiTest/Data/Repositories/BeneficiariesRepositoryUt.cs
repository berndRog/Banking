using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesRepositoryUt : BaseRepositoryUt{
   

   //#region methods beneficiary <-> account   
   [Fact]
   public async Task FindById() {
      // Arrange
      await _arrangeTest.Owner1With2AccountsAndBenficiariesAsync(_seed);
      var expected = _seed.Beneficiary1;
      // Act 
      var actual = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary1.Id);
      // Assert
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task AddBeneficiary(){
      // Arrange
      await _arrangeTest.Owner1With2AccountsAsync(_seed);
      var expected = _seed.Beneficiary1;
      
      // Act
      _seed.Account1.AddBeneficiary(_seed.Beneficiary1);
      _beneficiariesRepository.Add(_seed.Beneficiary1);
      await _dataContext.SaveAllChangesAsync("Add Beneficiary");
      
      // Assert
      var actual = await _beneficiariesRepository.FindByIdAsync(_seed.Beneficiary1.Id);
      Assert.Equivalent(expected, actual);
   }
   
   [Fact]
   public async Task SelectByAccountId() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      var expected = new List<Beneficiary> { _seed.Beneficiary1, _seed.Beneficiary2 };
      // Act 
      var actual = await _beneficiariesRepository.SelectByAccountIdAsync(_seed.Account1.Id);
      // Assert
      Assert.Equivalent(expected, actual);
      
      
   }
   //#endregion
  
}
