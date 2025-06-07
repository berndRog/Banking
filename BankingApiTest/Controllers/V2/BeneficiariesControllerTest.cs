using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dtos;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BeneficiariesControllerTest: BaseControllerTest {

   [Fact]
   public async Task GetBeneficiariesAsyncTest() {
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
   public async Task GetBeneficiariesByName() {
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
   public async Task GetBeneficiaryByIban() {
      // Arrange
      await _arrangeTest.OwnersWithAccountsAndBeneficiariesAsync(_seed);
      var iban = "DE40 10000000 0000000000";
      var expected = new List<BeneficiaryDto> {
         _seed.Beneficiary4.ToBeneficiaryDto(), 
         _seed.Beneficiary6.ToBeneficiaryDto()
      };

      // Act
      var actionResult = await _beneficiariesController.GetByIbanAsync(iban);

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
   
   [Fact]
   public async Task DeleteBeneficiaryTest() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      
      // Act
      var actionResult
         = await _beneficiariesController.DeleteAsync(_seed.Account1.Id, _seed.Beneficiary1.Id);
       
      // Assert
      Assert.IsType<NoContentResult>(actionResult);
   }
   
}