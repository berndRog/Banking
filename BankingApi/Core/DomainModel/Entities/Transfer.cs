namespace BankingApi.Core.DomainModel.Entities;

public class Transfer: IEntity<Guid>   {
   
   public Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date {get; private set; } = DateTime.UtcNow;
   public string Description { get; private set; } = string.Empty;
   public decimal Amount{ get; private set; }  // Amount < 0 -> Reverse SendMoney
   
   // debit account
   // navigation property Transfer (0,n):(1,1) Account  
   public Account Account { get; private set; } = null!;
   public Guid AccountId { get; private set; } 
   // navigation property Transfer (c):(0,1) Beneficiary
   public Beneficiary? Beneficiary { get; private set; }
   public Guid? BeneficiaryId { get; private set; }
   // navigation collection Transfer(0,1):(0,2) Transaction
   public IList<Transaction> Transactions { get; } = [];
   
   public Transfer() {}
   public Transfer(Guid? id, DateTime date, string description, decimal amount, 
      Guid? accountId = null, Guid? beneficiaryId = null) {
      if(id.HasValue) Id = id.Value;
      Date = date;
      Description = description;
      Amount = amount;
      if(accountId.HasValue)  AccountId = accountId.Value;
      if(beneficiaryId.HasValue) BeneficiaryId = beneficiaryId.Value;
   }
   
   public void SetAccount(Account account) {
      Account = account;
      AccountId = account.Id;
   }

   public void SetBeneficiary(Beneficiary? beneficiary) {
      if (beneficiary != null) {
         Beneficiary = beneficiary;
         BeneficiaryId = beneficiary.Id;
      }
      else {
         Beneficiary = null;
         BeneficiaryId = null;
      }
   }
   
   public void Add(Transaction transaction) =>
      Transactions.Add(transaction);

   public Transaction? RemoveTransaction() {
      var transaction = Transactions.FirstOrDefault(t => t.Amount < 0);  // Debit
      if(transaction == null) return null;
      transaction.Transfer = null;
      transaction.TransferId = Guid.Empty;
      return transaction;
   }
}