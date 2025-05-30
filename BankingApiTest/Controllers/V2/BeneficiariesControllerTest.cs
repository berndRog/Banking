using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesControllerTest: BaseControllerTest {

   public async Task GetAllAsyncTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Beneficiaries.Select(b => b.ToBeneficiaryDto());
      // Act
      var actionResult = await _beneficiariesController.GetAllAsync();
      // Assert
      THelper.IsEnumerableOk(actionResult!, expected);
   }
   
   [Fact]
   public async Task GetBeneficiariesByAccountId() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = new List<BeneficiaryDto> {
         _seed.Beneficiary1.ToBeneficiaryDto(),
         _seed.Beneficiary2.ToBeneficiaryDto(),
      };
      // Act
      var actionResult = 
         await _beneficiariesController.GetByAccountIdAsync(_seed.Account1.Id);
      
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   [Fact]
   public async Task GetBeneficiaryById() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var expected = _seed.Beneficiary1.ToBeneficiaryDto();
      // Act
      var actionResult
         = await _beneficiariesController.GetByIdAsync(_seed.Beneficiary1.Id);
      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }

   
   [Fact]
   public async Task GetBeneficiariesByName_Ok() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      var name = "Christ";
      var expected = new List<BeneficiaryDto> {
         _seed.Beneficiary1.ToBeneficiaryDto(), 
         _seed.Beneficiary2.ToBeneficiaryDto()
      };

      // Act
      var actionResult = await _beneficiariesController.GetByNameAsync(name);

      // Assert
      THelper.IsEnumerableOk(actionResult, expected);
   }
   
   
   [Fact]
   public async Task CreateBeneficiaryTest() {
       // Arrange
       await _arrangeTest.OwnersWithAccountsAsync(_seed);
       var beneficiary1Dto = _seed.Beneficiary1.ToBeneficiaryDto();
       var expected = beneficiary1Dto with { AccountId = _seed.Account1.Id };
       
       // Act
       var actionResult
          = await _beneficiariesController.CreateAsync(_seed.Account1.Id, beneficiary1Dto);
       
       // Assert
       THelper.IsCreated(actionResult, expected);
    }
   
}