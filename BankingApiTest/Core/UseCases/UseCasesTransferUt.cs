using BankingApiTest.Persistence.Repositories;
using Xunit;
namespace BankingApiTest.Core.UseCases;
[Collection(nameof(SystemTestCollectionDefinition))]
public class UseCasesTransferUt : BaseRepositoryUt {
  
   /*
   [Fact]
   public async Task SendMoneyAsyncUt() {
      // Arrange
      await _arrangeTest.PrepareTest1(_seed);
      var accountDebitId = _seed.Account1.Id;
      
      var date = _seed.Transfer1.Date;
      var description = _seed.Transfer1.Description;
      var amount = _seed.Transfer1.Amount;
      var accountDebit = _seed.Account1;
      var accountCredit = _seed.Account6;
      
      var transferDto = new TransferDto(
         Id: _seed.Transfer1.Id,
         Date: date,
         Description: description,
         Amount: amount,
         AccountId:  _seed.Account1.Id,
         BeneficiaryId: _seed.Beneficiary1.Id
      );
      var transfer = _mapper.Map<Transfer>(transferDto);
      
      // Act   
      var result = await _useCasesTransfer.SendMoneyAsync(
         accountDebitId,
         transfer
      );

      // Assert
      if (result is Error<Transfer>)
         Assert.Fail(result?.Message);
      var actualTransfer = result?.Data as Transfer;
      actualTransfer.Should().NotBeNull();
      actualTransfer!.Date.Should().Be(date);
      actualTransfer!.Description.Should().Be(description);

      actualTransfer!.Amount.Should().Be(amount);

      var transactions = await _transactionsRepository.SelectByTransferIdAsync(actualTransfer.Id);
      transactions.Should().NotBeNull();

      var transactionDebit = transactions.FirstOrDefault(t => t.Amount <= 0.0);
      var transactionCredit = transactions.FirstOrDefault(t => t.Amount > 0.0);

      transactionDebit.Should().NotBeNull();
      transactionDebit!.Amount.Should().Be(-amount);
      transactionDebit!.Date.Should().Be(date);
      transactionDebit!.AccountId.Should().Be(accountDebit!.Id);
      transactionDebit!.TransferId.Should().Be(actualTransfer.Id);

      transactionCredit.Should().NotBeNull();
      transactionCredit!.Amount.Should().Be(amount);
      transactionCredit!.Date.Should().Be(date);
      transactionCredit!.AccountId.Should().Be(accountCredit!.Id);
      transactionCredit!.TransferId.Should().Be(actualTransfer!.Id);

   }

   [Fact]
   public async Task ReverseMoneyAsyncUt() {
      // Arrange
      await _arrangeTest.SendMoneyTest1(_seed);
      var originalTransfer = _seed.Transfer1;
      var reverseTransferDto = new TransferDto(
         Id: Guid.NewGuid(),
         Date: DateTime.UtcNow, 
         Description: "Rückbuchung Erika Chris",
         Amount: -originalTransfer!.Amount,
         AccountId:  originalTransfer.AccountId,
         BeneficiaryId: (Guid) originalTransfer.BeneficiaryId!
      );
      var reverseTransfer = _mapper.Map<Transfer>(reverseTransferDto);
      
      // Act   
      var result = await _useCasesTransfer.ReverseMoneyAsync(
         originalTransfer.Id,
         reverseTransfer
      );

      // Assert
      if (result is Error<Transfer>)
         Assert.Fail(result.Message);

      var actualTransfer = result.Data as Transfer;
      actualTransfer.Should().NotBeNull();
      actualTransfer!.Description.Should().Be(reverseTransfer.Description);
      actualTransfer!.Date.Should().Be(reverseTransfer.Date);
      actualTransfer!.Amount.Should().Be(-originalTransfer.Amount);

      var reverseTransactions = await _transactionsRepository.SelectByTransferIdAsync(actualTransfer.Id);
      reverseTransactions.Should().NotBeNull();
      var reverseTransactionDebit = reverseTransactions.FirstOrDefault(t => t.Amount <= 0.0);
      var reverseTransactionCredit = reverseTransactions.FirstOrDefault(t => t.Amount > 0.0);
      reverseTransactionDebit.Should().NotBeNull();
      reverseTransactionDebit!.Amount.Should().Be(-originalTransfer.Amount);
      reverseTransactionDebit!.Date.Should().Be(reverseTransfer.Date);
      reverseTransactionDebit!.AccountId.Should().Be(_seed.Account6.Id);
 
      reverseTransactionCredit.Should().NotBeNull();
      reverseTransactionCredit!.Amount.Should().Be(originalTransfer.Amount);
      reverseTransactionCredit!.Date.Should().Be(reverseTransfer.Date);
      reverseTransactionCredit!.AccountId.Should().Be(_seed.Account1.Id);
      
   }
   */
}