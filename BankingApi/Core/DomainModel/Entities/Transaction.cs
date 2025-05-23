using System;
namespace BankingApi.Core.DomainModel.Entities;

public class Transaction: IEntity<Guid>   {

   public Guid Id { get; init; } = Guid.NewGuid();
   public DateTime Date { get; private set; } = DateTime.UtcNow;
   public decimal Amount{ get; private set; }
   
   // navigation property Transaction (0,n):(1,1) Account  
   public Account Account { get; private set; } = null!;
   public Guid AccountId { get; private set; }
   // navigation property Transaction (0,2):(0,1) Transfer  
   public Transfer? Transfer { get; set; }
   public Guid? TransferId { get; set; }
   
   public Transaction() {}
   public Transaction(Guid? id, DateTime date, decimal amount, 
      Guid? accountId = null, Guid? transferId = null) {
      if(id.HasValue) Id = id.Value;
      Date = date;
      Amount = amount;
      if(accountId.HasValue)  AccountId = accountId.Value;
      if(transferId.HasValue) TransferId = transferId.Value;
   }
   
   public void Set(Account account, Transfer transfer, bool isDebit = true){
      Account = account;
      AccountId = account.Id;
      Transfer = transfer;
      TransferId = transfer.Id;
      Date = transfer.Date;
      Amount = isDebit ? -transfer.Amount : transfer.Amount;
   }
}