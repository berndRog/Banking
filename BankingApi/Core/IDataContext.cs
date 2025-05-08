using System.Threading;
using System.Threading.Tasks;
namespace BankingApi.Core;

public interface IDataContext {
   // DbSet<Owner> Owners { get; }
   // DbSet<Account> Accounts { get; }
   // DbSet<Beneficiary> Beneficiaries { get; }
   // DbSet<Transfer> Transfers { get; }
   // DbSet<Transaction> Transactions { get; }
   // DbSet<Image> Images { get; }
   
   Task<bool> SaveAllChangesAsync(
      string? text = null,
      CancellationToken ctToken = default
   ); 
   void ClearChangeTracker();
   void LogChangeTracker(string text);
}