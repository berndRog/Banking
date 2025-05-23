using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.Entities;

public class Account: IEntity<Guid> {

   public Guid Id { get; init; } = Guid.NewGuid();
   public string Iban { get; private set; } = string.Empty;
      //init => _iban = Utils.CheckIban(value);
   public decimal Balance { get; private set; } 
   
   // navigation Property Account (0,n):(1,1) Owner  
   public Owner Owner { get; private set; } = null!;
   public Guid OwnerId{ get; private set; } 
   // navigation collection Account (c):(0,n) Beneficiary  
   public IList<Beneficiary> Beneficiaries { get; private set; } = [];
   // navigation collection Account (1,1):(0,n) Transfer 
   public IList<Transfer> Transfers { get; private set; } = [];
   // navigation collectiony Account (1,1):(0,n) Transaction  
   public IList<Transaction> Transactions { get; private set; } = [];
   
   public Account() {}
   public Account(Guid? id, string iban, decimal balance, Guid? ownerId = null) {
      if(id.HasValue) Id = id.Value;
      Iban = Utils.CheckIban(iban);
      Balance = balance;
      if(ownerId.HasValue) OwnerId = ownerId.Value;
   }

   public void SetOwner(Owner owner) {
      Owner = owner;
      OwnerId = owner.Id;
   }
   
   public void AddBeneficiary(Beneficiary beneficiary){  
      // set account in beneficiary
      beneficiary.SetAccount(this);
      Beneficiaries.Add(beneficiary);
   }

   // Add transfer to account and to beneficiary
   public void AddTransfer(Transfer transfer, Beneficiary beneficiary) {
      // set account in transfer
      transfer.SetAccount(this);
      Transfers.Add(transfer);
      // set beneficiary in transfer 
      transfer.SetBeneficiary(beneficiary);
   }
   
   // Add transactions to accounts
   public void AddTransactions(Transaction transaction, Transfer transfer, bool isDebit){
      
      // set account and Transfer to transaction
      transaction.Set(this, transfer, isDebit);
      // add transaction to transfer
      transfer.Add(transaction);
      
      // add transaction to account
      Transactions.Add(transaction);
      
      // change account balance
      Balance = Balance + transaction.Amount;
   }
}