using BankingApi.Core.Misc;
namespace BankingApi.Core.DomainModel.Entities; 

public class Beneficiary: IEntity<Guid>{
   
   public Guid Id { get; init; } = Guid.NewGuid();
   // name of the credit account owner
   public string Name { get; private set; } = string.Empty;  
   // transfer money to credit account via Iban
   public string Iban { get; private set; } = string.Empty;
   // navigation property Beneficiary (c):(1,1) Account
   // Debit account
   public Guid AccountId{ get; private set; }  
   
   // ctor EF Core
   public Beneficiary() {}
   // ctor DomainModel
   public Beneficiary(Guid? id, string name, string iban, Guid? accountId = null) {
      if(id.HasValue) Id = id.Value;
      Name = name;
      Iban = Utils.CheckIban(iban);
      // Debit account
      if(accountId.HasValue)  AccountId = accountId.Value;
   }
   // Beneficiary (0,n):(1,1) Account 
   public void SetAccount(Account account) {
      AccountId = account.Id;
   }
   
}