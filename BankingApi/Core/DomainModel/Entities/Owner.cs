namespace BankingApi.Core.DomainModel.Entities;

public class Owner: IEntity<Guid> {

   public Guid Id { get; init; } = Guid.NewGuid();
   public string Name { get; private set; } = string.Empty;
   public DateTime Birthdate { get; private set; } = DateTime.UtcNow;
   public string? Email { get; private set; } = null;
   // navigation collection Owner (1,1):(0,n) Account  
   public IList<Account> Accounts { get; } = [];
   
   public Owner() {}
   public Owner(Guid? id, string name, DateTime birthdate, string? email) {
      if(id.HasValue) Id = id.Value;
      Name = name;
      Birthdate = birthdate;
      if(email != null)  Email = email;
   }
   
   public void Update(string? name, string? email) {
      if(!string.IsNullOrEmpty(name)) Name = name;
      if(!string.IsNullOrEmpty(email)) Email = email;
   } 
   
   public void AddAccount(Account account) {
      account.SetOwner(this);
      Accounts.Add(account);
   }   
   public void RemoveAccount(Account account) {
      Accounts.Remove(account);
   }   
}