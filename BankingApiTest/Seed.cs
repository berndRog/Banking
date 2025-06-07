using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
namespace BankingApiTest;

public class Seed {

   #region fields
   public Owner Owner1{ get; }
   public Owner Owner2{ get; }
   public Owner Owner3{ get; }
   public Owner Owner4{ get; }
   public Owner Owner5{ get; }
   public Owner Owner6{ get; }

   public Account Account1{ get; }
   public Account Account2{ get; }
   public Account Account3{ get; }
   public Account Account4{ get; }
   public Account Account5{ get; }
   public Account Account6{ get; }
   public Account Account7{ get; }
   public Account Account8{ get; }

   public Beneficiary Beneficiary1{ get; }
   public Beneficiary Beneficiary2{ get; }
   public Beneficiary Beneficiary3{ get; }
   public Beneficiary Beneficiary4{ get; }
   public Beneficiary Beneficiary5{ get; }
   public Beneficiary Beneficiary6{ get; }
   public Beneficiary Beneficiary7{ get; }
   public Beneficiary Beneficiary8{ get; }
   public Beneficiary Beneficiary9{ get; }
   public Beneficiary Beneficiary10{ get; }
   public Beneficiary Beneficiary11{ get; }

   public Transfer Transfer1{ get;}
   public Transfer Transfer2{ get; }
   public Transfer Transfer3{ get; }
   public Transfer Transfer4{ get; }
   public Transfer Transfer5{ get; }
   public Transfer Transfer6{ get; }
   public Transfer Transfer7{ get; }
   public Transfer Transfer8{ get; }
   public Transfer Transfer9{ get; }
   public Transfer Transfer10{ get; }
   public Transfer Transfer11{ get; }

   public Transaction Transaction1{ get; }
   public Transaction Transaction2{ get; }
   public Transaction Transaction3{ get; }
   public Transaction Transaction4{ get; }
   public Transaction Transaction5{ get; }
   public Transaction Transaction6{ get; }
   public Transaction Transaction7{ get; }
   public Transaction Transaction8{ get; }
   public Transaction Transaction9{ get; }
   public Transaction Transaction10{ get; }
   public Transaction Transaction11{ get; }
   public Transaction Transaction12{ get; }
   public Transaction Transaction13{ get; }
   public Transaction Transaction14{ get; }
   public Transaction Transaction15{ get; }
   public Transaction Transaction16{ get; }
   public Transaction Transaction17{ get; }
   public Transaction Transaction18{ get; }
   public Transaction Transaction19{ get; }
   public Transaction Transaction20{ get; }
   public Transaction Transaction21{ get; }
   public Transaction Transaction22{ get; }
   
   public List<Owner> Owners{ get; private set; }
   public List<Account> Accounts{ get; private set; } 
   public List<Beneficiary> Beneficiaries{ get; private set; } 
   public List<Transfer> Transfers{ get; private set; }
   public List<Transaction> Transactions{ get; private set; } 
   #endregion

   public Seed(){
      #region Owners
      Owner1 = new Owner(
         id: new Guid("10000000-0000-0000-0000-000000000000"),
         name: "Erika Mustermann",
         birthdate: new DateTime(1988, 2, 1).ToUniversalTime(),
         email: "erika.mustermann@t-online.de"
      );
      Owner2 = new Owner(
         id: new Guid("20000000-0000-0000-0000-000000000000"),
         name: "Max Mustermann",
         birthdate: new DateTime(1980, 12, 31).ToUniversalTime(),
         email: "max.mustermann@gmail.com"
      );
      Owner3 = new Owner(
         id: new Guid("30000000-0000-0000-0000-000000000000"),
         name: "Arno Arndt",
         birthdate: new DateTime(1969, 04, 11).ToUniversalTime(),
         email: "a.arndt@t-online.com"
      );
      Owner4 = new Owner(
         id: new Guid("40000000-0000-0000-0000-000000000000"),
         name: "Benno Bauer",
         birthdate: new DateTime(1965, 05, 18).ToUniversalTime(),
         email: "b.bauer@gmail.com"
      );
      Owner5 = new Owner(
         id: new Guid("50000000-0000-0000-0000-000000000000"),
         name: "Christine Conrad",
         birthdate: new DateTime(1972, 06, 07).ToUniversalTime(),
         email: "c.conrad@gmx.de"
      );
      Owner6 = new Owner(
         id: new Guid("60000000-0000-0000-0000-000000000000"),
         name: "Dana Deppe",
         birthdate: new DateTime(1978, 09, 11).ToUniversalTime(),
         email: "d.deppe@icloud.com"
      );
      #endregion

      #region Accounts
      Account1 = new Account(
         id: new Guid("01000000-0000-0000-0000-000000000000"),
         iban: "DE10 1000 0000 0000 0000 00",
         balance: 2100.0m
      );
      Account2 = new Account(
         id: new Guid("02000000-0000-0000-0000-000000000000"),
         iban: "DE10 2000 0000 0000 0000 00",
         balance: 2000.0m
      );
      Account3 = new Account(
         id: new Guid("03000000-0000-0000-0000-000000000000"),
         iban: "DE20 1000 0000 0000 0000 00",
         balance: 3000.0m
      );
      Account4 = new Account(
         id: new Guid("04000000-0000-0000-0000-000000000000"),
         iban: "DE30 1000 0000 0000 0000 00",
         balance:  2500.0m
      );
      Account5 = new Account(
         id: new Guid("05000000-0000-0000-0000-000000000000"),
         iban: "DE40 1000 0000 0000 0000 00",
         balance: 1900.0m
      );
      Account6 = new Account(
         id: new Guid("06000000-0000-0000-0000-000000000000"),
         iban: "DE50 1000 0000 0000 0000 00",
         balance: 3500.0m
      );
      Account7 = new Account(
         id: new Guid("07000000-0000-0000-0000-000000000000"),
         iban: "DE50 2000 0000 0000 0000 00",
         balance: 3100.0m
      );
      Account8 = new Account(
         id: new Guid("08000000-0000-0000-0000-000000000000"),
         iban: "DE60 1000 0000 0000 0000 00",
         balance: 4300.0m
      );
      #endregion

      #region Beneficiaries
      Beneficiary1 = new Beneficiary(
         id: new Guid("00100000-0000-0000-0000-000000000000"),
         name: Owner5.Name,
         iban: Account6.Iban
      );
      Beneficiary2 = new Beneficiary(
         id: new Guid("00200000-0000-0000-0000-000000000000"),
         name: Owner5.Name,
         iban: Account7.Iban
      );
      Beneficiary3 = new Beneficiary(
         id: new Guid("00300000-0000-0000-0000-000000000000"),
         name: Owner3.Name,
         iban: Account4.Iban
      );
      Beneficiary4 = new Beneficiary(
         id: new Guid("00400000-0000-0000-0000-000000000000"),
         name: Owner4.Name,
         iban: Account5.Iban
      );
      Beneficiary5 = new Beneficiary(
         id: new Guid("00500000-0000-0000-0000-000000000000"),
         name: Owner3.Name,
         iban: Account4.Iban
      );
      Beneficiary6 = new Beneficiary(
         id: new Guid("00600000-0000-0000-0000-000000000000"),
         name: Owner4.Name,
         iban: Account5.Iban
      );
      Beneficiary7 = new Beneficiary(
         id: new Guid("00700000-0000-0000-0000-000000000000"),
         name: Owner6.Name,
         iban: Account8.Iban
      );
      Beneficiary8 = new Beneficiary(
         id: new Guid("00800000-0000-0000-0000-000000000000"),
         name: Owner2.Name,
         iban: Account3.Iban
      );
      Beneficiary9 = new Beneficiary(
         id: new Guid("00900000-0000-0000-0000-000000000000"),
         name: Owner5.Name,
         iban: Account7.Iban
      );
      Beneficiary10 = new Beneficiary(
         id: new Guid("01000000-0000-0000-0000-000000000000"),
         name: Owner1.Name,
         iban: Account1.Iban
      );
      Beneficiary11 = new Beneficiary(
         id: new Guid("01100000-0000-0000-0000-000000000000"),
         name: Owner1.Name,
         iban: Account2.Iban
      );
      #endregion

      #region Transfers
      Transfer1 = new Transfer(
         id: new Guid("00010000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 01, 01, 08, 00, 00).ToUniversalTime(),
         description: "Erika an Chris1",
         amount: 345.0m
      );
      Transfer2 = new Transfer(
         id: new Guid("00020000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 02, 01, 09, 00, 00).ToUniversalTime(),
         description: "Erika an Chris2",
         amount: 231.0m
      );
      Transfer3 = new Transfer(
         id: new Guid("00030000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 03, 01, 10, 00, 00).ToUniversalTime(),
         description: "Erika an Arne",
         amount: 289.00m
      );
      Transfer4 = new Transfer(
         id: new Guid("00040000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 04, 01, 11, 00, 00).ToUniversalTime(),
         description: "Erika an Benno",
         amount: 125.0m
      );
      Transfer5 = new Transfer(
         id: new Guid("00050000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 05, 01, 12, 00, 00).ToUniversalTime(),
         description: "Max an Arne",
         amount: 167.0m
      );
      Transfer6 = new Transfer(
         id: new Guid("00060000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 06, 01, 13, 00, 00).ToUniversalTime(),
         description: "Max an Benno",
         amount: 289.0m
      );
      Transfer7 = new Transfer(
         id: new Guid("00070000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 07, 01, 14, 00, 00).ToUniversalTime(),
         description: "Max an Dana",
         amount: 312.0m
      );
      Transfer8 = new Transfer(
         id: new Guid("00080000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 08, 01, 15, 00, 00).ToUniversalTime(),
         description: "Arne an Max",
         amount: 278.0m
      );
      Transfer9 = new Transfer(
         id: new Guid("00090000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 09, 01, 16, 00, 00).ToUniversalTime(),
         description: "Arne an Christ2",
         amount: 356.0m
      );
      Transfer10 = new Transfer(
         id: new Guid("00100000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 10, 01, 17, 00, 00).ToUniversalTime(),
         description: "Benno an Erika1",
         amount: 412.0m
      );
      Transfer11 = new Transfer(
         id: new Guid("00110000-0000-0000-0000-000000000000"),
         date: new DateTime(2023, 11, 01, 18, 00, 00).ToUniversalTime(),
         description: "Benno an Erika2",
         amount: 89.0m
      );
      #endregion

      #region Transaction
      var date = new DateTime(2000, 01, 01, 12, 00, 00).ToUniversalTime();
      Transaction1 = new Transaction(
         id: new Guid("00001000-0000-0000-0000-000000000000"),
         date: date,
         amount: 0m
      );
      Transaction2 = new Transaction(
         id: new Guid("00002000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction3 = new Transaction(
         id: new Guid("00003000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction4 = new Transaction(
         id: new Guid("00004000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction5 = new Transaction(
         id: new Guid("00005000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction6 = new Transaction(
         id: new Guid("00006000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction7 = new Transaction(
         id: new Guid("00007000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction8 = new Transaction(
         id: new Guid("00008000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction9 = new Transaction(
         id: new Guid("00009000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction10 = new Transaction(
         id: new Guid("00010000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction11 = new Transaction(
         id: new Guid("00011000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction12 = new Transaction(
         id: new Guid("00012000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction13 = new Transaction(
         id: new Guid("00013000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction14 = new Transaction(
         id: new Guid("00014000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction15 = new Transaction(
         id: new Guid("00015000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction16 = new Transaction(
         id: new Guid("00016000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction17 = new Transaction(
         id: new Guid("00017000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction18 = new Transaction(
         id: new Guid("00018000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction19 = new Transaction(
         id: new Guid("00019000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction20 = new Transaction(
         id: new Guid("00020000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction21 = new Transaction(
         id: new Guid("00021000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      Transaction22 = new Transaction(
         id: new Guid("00022000-0000-0000-0000-000000000000"),      
         date: date,
         amount: 0m
      );
      #endregion

      Owners = [Owner1, Owner2, Owner3, Owner4, Owner5, Owner6];
      Accounts = [Account1, Account2, Account3, Account4, Account5, Account6, Account7, Account8];
      Beneficiaries = [
         Beneficiary1, Beneficiary2, Beneficiary3, Beneficiary4, Beneficiary5,
         Beneficiary6, Beneficiary7, Beneficiary8, Beneficiary9, Beneficiary10, Beneficiary11
      ];
      Transfers = [
         Transfer1, Transfer2, Transfer3, Transfer4, Transfer5, Transfer6, Transfer7,
         Transfer8, Transfer9, Transfer10, Transfer11
      ];
      Transactions = [
         Transaction1, Transaction2, Transaction3, Transaction4, Transaction5,
         Transaction6, Transaction7, Transaction8, Transaction9, Transaction10, Transaction11, Transaction12,
         Transaction13, Transaction14, Transaction15, Transaction16, Transaction17, Transaction18, Transaction19,
         Transaction20, Transaction21, Transaction22
      ];
   }
   
   // Setup Relations between Owners and Accounts
   public Seed InitAccounts(){
      Owner1.AddAccount(Account1); // Owner 1 with 2 accounts 1+2
      Owner1.AddAccount(Account2);
      Owner2.AddAccount(Account3); // Owner 2 witn account 3
      Owner3.AddAccount(Account4); // Owner 3 with account 4
      Owner4.AddAccount(Account5); // Owner 4 with account 5
      Owner5.AddAccount(Account6); // Owner 5 with 2 accounts 6+7
      Owner5.AddAccount(Account7);
      Owner6.AddAccount(Account8); // Owner 6 wiht account 8
      return this;
   }
   
   // Setup Relations between Owner1 and Account1, Account2
   public Seed InitAccountsForOwner1(){
      Owner1.AddAccount(Account1); // Owner 1 with 2 accounts 1+2
      Owner1.AddAccount(Account2);
      return this;
   }

   // Setup Relations between Accounts and Beneficiaries
   public Seed InitBeneficiaries(){
      // Accounts
      Account1.AddBeneficiary(Beneficiary1); // Account 1
      Account1.AddBeneficiary(Beneficiary2);
      Account2.AddBeneficiary(Beneficiary3); // Account 2
      Account2.AddBeneficiary(Beneficiary4);
      Account3.AddBeneficiary(Beneficiary5); // Account 3
      Account3.AddBeneficiary(Beneficiary6);
      Account3.AddBeneficiary(Beneficiary7);
      Account4.AddBeneficiary(Beneficiary8); // Account 4
      Account4.AddBeneficiary(Beneficiary9);
      Account5.AddBeneficiary(Beneficiary10); // Account 5
      Account5.AddBeneficiary(Beneficiary11);
      return this;
   }

   public Seed InitBeneficiariesForAccoutsOfOwner1(){
      Account1.AddBeneficiary(Beneficiary1); // Account 1
      Account1.AddBeneficiary(Beneficiary2);
      Account2.AddBeneficiary(Beneficiary3); // Account 2
      Account2.AddBeneficiary(Beneficiary4);
      return this;
   }

   public Seed InitTransfersTransactions(){
      // Traansfer 1-4
      SendMoney(Account1, Beneficiary1, Transfer1, Transaction1, Transaction2); // From Account 1
      SendMoney(Account1, Beneficiary2, Transfer2, Transaction3, Transaction4);
      SendMoney(Account2, Beneficiary3, Transfer3, Transaction5, Transaction6); // From Account 2
      SendMoney(Account2, Beneficiary4, Transfer4, Transaction7, Transaction8);
      // Transfers 5-7
      SendMoney(Account3, Beneficiary5, Transfer5, Transaction9, Transaction10); // From Account 3
      SendMoney(Account3, Beneficiary6, Transfer6, Transaction11, Transaction12);
      SendMoney(Account3, Beneficiary7, Transfer7, Transaction13, Transaction14);
      // Transfers 8-9
      SendMoney(Account4, Beneficiary8, Transfer8, Transaction15, Transaction16); // From Account 4
      // hier titt der fehler auf
      SendMoney(Account4, Beneficiary9, Transfer9, Transaction17, Transaction18);
      // Transfers 10-11
      SendMoney(Account5, Beneficiary10, Transfer10, Transaction19, Transaction20); // From Account 5
      SendMoney(Account5, Beneficiary11, Transfer11, Transaction21, Transaction22);

      return this;
   }

   public void SendMoney(
      Account accountDebit,
      Beneficiary beneficiary,
      Transfer transfer,
      Transaction transactionDebit,
      Transaction transactionCredit
   ) {
      var accountCredit = Accounts.Find(a => a.Iban == beneficiary.Iban);
      if (accountCredit == null) {
         throw new ArgumentException("Account for beneficiary not found: " + beneficiary.Iban);
      }
      
      accountDebit.AddTransfer(transfer, beneficiary);
      
      transactionDebit.Set(accountDebit, transfer, true);
      transactionCredit.Set(accountCredit, transfer, false);
      
//      transactionDebit?.Account?.AddTransactions(transactionDebit, transfer, true);
//      transactionCredit?.Account?.AddTransactions(transactionCredit, transfer, false);
      // add transaction to transfer and debit account (Lastschrift)
      accountDebit.AddTransactions(transactionDebit, transfer, true);
      // add transaction to transfer and credit account (Gutschrift)     
      accountCredit.AddTransactions(transactionCredit, transfer, false);

   }

   public void PrepareTest1(){ 
      Owner1.AddAccount(Account1);
      Owner5.AddAccount(Account6);
      // AccountDebit
      Account1.AddBeneficiary(Beneficiary1);
      // Account1.Add(Transfer1, Beneficiary1);
      // Account1.Add(Transaction1, Transfer1);
      // // AccountCredit
      // Account6.Add(Transaction2, Transfer1);
   }
   
   public void DoTransfer1(){
      Owner1.AddAccount(Account1);
      Owner5.AddAccount(Account6);
      // AccountDebit
      Account1.AddBeneficiary(Beneficiary1);
      
      SendMoney(Account1, Beneficiary1, Transfer1, Transaction1, Transaction2); 
   }
   
   public void PrepareExample1WithReverse(DateTime reverseDate){
      Owner1.AddAccount(Account1);
      Owner5.AddAccount(Account6);
      // AccountDebit
      Account1.AddBeneficiary(Beneficiary1);
      Account1.AddTransfer(Transfer1, Beneficiary1);
      Account1.AddTransactions(Transaction1, Transfer1, true);
      // AccountCredit
      Account6.AddTransactions(Transaction2, Transfer1, false);

      Transfer transfer = new(
         id: new Guid("90010000-0000-0000-0000-000000000000"),
         description: "Reverse " + Transfer1.Description,
         date: reverseDate,
         amount:-Transfer1.Amount
      );

      Transaction transactionDebit = new(
         id: new Guid("90001000-0000-0000-0000-000000000000"),
         date: Transfer1.Date,
         amount: Transfer1.Amount
      );
      Transaction transactionCredit = new(
         id: new Guid("90002000-0000-0000-0000-000000000000"),
         date: Transfer1.Date,
         amount:-Transfer1.Amount
      );
      
      // Reverse (Debit) - Lastschrift
      Account1.AddTransfer(transfer, Beneficiary1);
      Account1.AddTransactions(transactionDebit, transfer, true);
      // Reverse (Credit)  - Gutschrift     
      Account6.AddTransactions(transactionCredit, transfer, false);
   }

   public void Example1(){
      Owner1.AddAccount(Account1);
      Owner5.AddAccount(Account6);
      Account1.AddBeneficiary(Beneficiary1); // beneficiary 1 for account 1
      SendMoney(Account1, Beneficiary1, Transfer1, Transaction1, Transaction2); // From Account 1
   }

   // public void Example2(){
   //    Owner1.AddAccount(Account1); // owner 1 with 2 accounts 1+2
   //    Owner1.Add(Account2);
   //    Owner5.Add(Account6); // owner 5 with 2 accounts 6+7
   //    Owner5.Add(Account7);
   //    Account1.Add(Beneficiary1); // beneficiary 1+2 for account 1
   //    Account1.Add(Beneficiary2);
   //    Account2.Add(Beneficiary3); // beneficiary 3+4 for account 2
   //    Account2.Add(Beneficiary4);
   //    SendMoney(Account1, Beneficiary1, Account6,
   //       Transfer1, Transaction1, Transaction2); // From Account 1
   //    SendMoney(Account1, Beneficiary2, Account7,
   //       Transfer2, Transaction3, Transaction4);
   // }

}