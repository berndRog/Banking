using System;
using System.Threading.Tasks;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Xunit;

namespace BankingApiTest.Persistence;
public class ArrangeTest(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   ITransactionsRepository transactionsRepository,
   IDataContext dataContext,
   IUseCasesTransfer useCasesTransfer
) {
   
   public async Task OwnerWith1AccountAsync(Seed seed){
      // Arrange
      // Owner owner = new() {
      //    Id = new Guid("10000000-0000-0000-0000-000000000000"),
      //    Name = "Erika Mustermann",
      //    Birthdate = new DateTime(1988, 2, 1).ToUtcDateTime(),
      //    Email = "erika.mustermann@t-online.de"
      // }; // _seed.Owner1
      // Account account = new() {
      //    Id = new Guid("01000000-0000-0000-0000-000000000000"),
      //    Iban = "DE10 10000000 0000000001",
      //    Balance = 2100.0
      // }; // _seed.Account1
      // owner.Add(account); // DomainModel
      // accountsRepository.Add(account);
      // await dataContext.SaveAllChangesAsync();
      seed.Owner1.AddAccount(seed.Account1);
      accountsRepository.Add(seed.Account1);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task<Owner> Owner1With2AccountsAsync(Seed seed) {
      // add owner1 to database
      var owner = seed.Owner1;
      ownersRepository.Add(owner);
      await dataContext.SaveAllChangesAsync("Add Owner");
      dataContext.ClearChangeTracker();
      
      // add the two accounts to the database
      var actualOwner = await ownersRepository.FindByIdAsync(owner.Id);
      Assert.NotNull(actualOwner);
      var account1 = seed.Account1;
      var account2 = seed.Account2;
      actualOwner.AddAccount(seed.Account1);
      actualOwner.AddAccount(seed.Account2);
      accountsRepository.Add(account1);
      accountsRepository.Add(account2);
      await dataContext.SaveAllChangesAsync("PrepareTest");
      dataContext.ClearChangeTracker();

      return actualOwner;
   }

   public async Task Owner1With2AccountsAndBenficiariesAsync(Seed seed) {
      // add owner1 to database
      var owner = seed.Owner1;
      ownersRepository.Add(owner);
      await dataContext.SaveAllChangesAsync("Add Owner1");
      dataContext.ClearChangeTracker();
      
      // add the two accounts to the database
      var actualOwner = await ownersRepository.FindByIdAsync(owner.Id);
      Assert.NotNull(actualOwner);
      var account1 = seed.Account1;
      var account2 = seed.Account2;
      actualOwner.AddAccount(seed.Account1);
      actualOwner.AddAccount(seed.Account2);
      accountsRepository.Add(account1);
      accountsRepository.Add(account2);
      await dataContext.SaveAllChangesAsync("Add Account1 + Account2");
      dataContext.ClearChangeTracker();
      
      var actualAccount1 = await accountsRepository.FindByIdAsync(account1.Id);
      Assert.NotNull(actualAccount1);
      actualAccount1.AddBeneficiary(seed.Beneficiary1);
      actualAccount1.AddBeneficiary(seed.Beneficiary2);
      beneficiariesRepository.Add(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary2);
      await dataContext.SaveAllChangesAsync("Add Beneficiary1 + Beneficiary2");
      dataContext.ClearChangeTracker();

      var actualAccount2 = await accountsRepository.FindByIdAsync(account2.Id);
      Assert.NotNull(actualAccount2);
      actualAccount2.AddBeneficiary(seed.Beneficiary3);
      actualAccount2.AddBeneficiary(seed.Beneficiary4);
      beneficiariesRepository.Add(seed.Beneficiary3);
      beneficiariesRepository.Add(seed.Beneficiary4);
      await dataContext.SaveAllChangesAsync("Add Beneficiary3 + Beneficiary4");
      dataContext.ClearChangeTracker();
   }
   
   public async Task OwnersWithAccountsAsync(Seed seed){
      seed.InitAccounts();
      ownersRepository.AddRange(seed.Owners);  
      accountsRepository.AddRange(seed.Accounts);
      dataContext.LogChangeTracker("OwnersWithAccountsAsync()");
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task OwnersWithAccountsAndBeneficiariesAsync(Seed seed){
      seed.InitAccounts();
      seed.InitBeneficiaries();

      ownersRepository.AddRange(seed.Owners);
      accountsRepository.AddRange(seed.Accounts);
      beneficiariesRepository.AddRange(seed.Beneficiaries);
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task PrepareTest1(Seed seed){
      
      // add owner1, owner5 to ownersRepository
      ownersRepository.Add(seed.Owner1);
      ownersRepository.Add(seed.Owner5);

      // add account1, account6 to accountsRepository
      seed.Owner1.AddAccount(seed.Account1);
      seed.Owner5.AddAccount(seed.Account6);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account6);

      // add beneficiary1 to beneficiariesRepository
      seed.Account1.AddBeneficiary(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary1);

      // save to database
      await dataContext.SaveAllChangesAsync();
      dataContext.ClearChangeTracker();
   }
   
   public async Task SendMoneyTest1(Seed seed){
      ownersRepository.Add(seed.Owner1);
      ownersRepository.Add(seed.Owner5);

      seed.Owner1.AddAccount(seed.Account1);
      seed.Owner5.AddAccount(seed.Account6);
      accountsRepository.Add(seed.Account1);
      accountsRepository.Add(seed.Account6);

      seed.Account1.AddBeneficiary(seed.Beneficiary1);
      beneficiariesRepository.Add(seed.Beneficiary1);
      await dataContext.SaveAllChangesAsync();

      seed.Transfer1.SetAccount(seed.Account1);
      seed.Transfer1.SetBeneficiary(seed.Beneficiary1);

      var result = await useCasesTransfer.SendMoneyAsync(
         accountDebitId:   seed.Account1.Id,
         transfer:         seed.Transfer1
      ); 
      if(result is Error<Transfer>) throw new Exception(result.Message);
      dataContext.ClearChangeTracker();
   }

   public async Task ExampleAsync(Seed seed){
      ownersRepository.AddRange(seed.Owners);
      await dataContext.SaveAllChangesAsync("Add Owners");

      seed.InitAccounts();
      accountsRepository.AddRange(seed.Accounts);
      await dataContext.SaveAllChangesAsync("Add Accounts");
      
      seed.InitBeneficiaries();
      beneficiariesRepository.AddRange(seed.Beneficiaries);
      await dataContext.SaveAllChangesAsync("Add Beneficieries");
      
      seed.InitTransfersTransactions();
      transfersRepository.AddRange(seed.Transfers);
      transactionsRepository.AddRange(seed.Transactions);
      await dataContext.SaveAllChangesAsync("Add Transfers with Transactions");

      dataContext.ClearChangeTracker();      
   }
}