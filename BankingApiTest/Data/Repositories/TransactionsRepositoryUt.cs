using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public  class TransactionsRepositoryUt: BaseRepositoryUt {
   
   /*
   private void ShowRepository(string text){
#if DEBUG
      _dataContext.LogChangeTracker(text);
#endif
   }

   private EquivalencyAssertionOptions<Transaction> ExcludeReferences(
      EquivalencyAssertionOptions<Transaction> options
   ) {
      options.Excluding(t => t.Account);
      options.Excluding(t => t.Transfer);
      return options;
   }
   
   [Fact]
   public async Task FindByIdUt() {
      // Arrange
      await _arrangeTest.ExampleAsync(_seed);
      // Act 
      var actual = await _transactionsRepository.FindByIdAsync(_seed.Transaction1.Id);
      // Assert
      ShowRepository("FindbyId");
      actual.Should().BeEquivalentTo(_seed.Transaction1, ExcludeReferences);
   }
   
   */
}