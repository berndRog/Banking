using System.Diagnostics;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data;

public class DataContext: DbContext, IDataContext {

 // public class DataContext(
 //    DbContextOptions<DataContext> options
 // ) : DbContext(options), IDataContext {
 //   

   //private readonly IConfiguration _configuration;
   private ILogger<DataContext>? _logger;

   public DbSet<Owner> Owners => Set<Owner>(); // call to a method, not a field 
   public DbSet<Account> Accounts => Set<Account>();
   public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
   public DbSet<Transfer> Transfers => Set<Transfer>();
   public DbSet<Transaction> Transactions => Set<Transaction>();
   public DbSet<Image> Images => Set<Image>();

   
   public DataContext(
      DbContextOptions<DataContext> options
   ) : base(options) {
//      Console.WriteLine($"....: DatabaseConfiguration: Sqlite foreign keys on");
//      Database.EnsureCreated();
//      Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON;");
   }
   
   
   protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      // https://learn.microsoft.com/de-de/ef/core/logging-events-diagnostics/simple-logging
      var loggerFactory = LoggerFactory.Create(builder => {
         builder
            .SetMinimumLevel(LogLevel.Information)
            .AddDebug()
            .AddConsole();
      });
      _logger = loggerFactory.CreateLogger<DataContext>();

      // Configure logging
      optionsBuilder
         .UseLoggerFactory(loggerFactory)
         .LogTo(Console.WriteLine, LogLevel.Information)
         .LogTo(message => Debug.WriteLine(message), LogLevel.Information)
         .EnableSensitiveDataLogging(true)
         .EnableDetailedErrors();
   }

   /*
   protected override void OnModelCreating(ModelBuilder modelBuilder) {
      
      base.OnModelCreating(modelBuilder);


      
      //
      // PROPERTY CONFIGURATION
      //
      modelBuilder.Entity<Owner>(e => {
         e.ToTable("Owners"); // tablename Owners
         e.HasKey(owner => owner.Id); // primary key
         e.Property(owner => owner.Id) // primary key has type Guid
            .ValueGeneratedNever(); // and should never be gerated by DB  
         e.Property(owner => owner.Name).HasMaxLength(100);
         e.Property(owner => owner.Email).HasMaxLength(200);
         e.Property(e => e.Birthdate)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });
      
      modelBuilder.Entity<Account>(e => {
         e.ToTable("Accounts");
         e.HasKey(account => account.Id);
         e.Property(account => account.Id) 
            .ValueGeneratedNever();        // Id should never be gerated by DB  
         e.Property(account => account.Id).ValueGeneratedNever();
         e.Property(account => account.Iban).HasMaxLength(32);
      });

      modelBuilder.Entity<Beneficiary>(e => {
         e.ToTable("Beneficiaries");
         e.HasKey(beneficiary => beneficiary.Id);
         e.Property(beneficiary => beneficiary.Id)
            .ValueGeneratedNever();        // Id and should never be gerated by DB  
         e.Property(beneficiary => beneficiary.Id).ValueGeneratedNever();
         e.Property(beneficiary => beneficiary.Name).HasMaxLength(64);
         e.Property(beneficiary => beneficiary.Iban).HasMaxLength(32);
      });
      
      modelBuilder.Entity<Transfer>(e => {
         e.ToTable("Transfers");
         e.HasKey(transfer => transfer.Id);
         e.Property(beneficiary => beneficiary.Id)
            .ValueGeneratedNever();        // Id and should never be gerated by DB  
         e.Property(transfer => transfer.Id).ValueGeneratedNever();
         e.Property(e => e.Date)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });
      
      modelBuilder.Entity<Transaction>(e => {
         e.ToTable("Transactions");
         e.HasKey(transaction => transaction.Id);
         e.Property(transaction => transaction.Id)
            .ValueGeneratedNever();
         e.Property(e => e.Date)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });
      
      
      modelBuilder.Entity<Image>(entity => {
         entity.Property(e => e.Updated)
            .HasConversion(
               v => v, // to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
      });

      //
      // RELATIONS
      //
      // One-to-Many: Owner (1,1):(0..n] Account 
      // -------------------------------------------
      modelBuilder.Entity<Account>(e => {
         e.HasOne(account => account.Owner)              // Account --> Owner   [1]
            .WithMany(owner => owner.Accounts)           // Owner   --> Account [0..*]
            .HasForeignKey(account => account.OwnerId)   // Fk in Account
            .HasPrincipalKey(owner => owner.Id)          // Pk in Owner
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
         e.Navigation(account => account.Owner);         // navigation property
      });
      //
      // One-To-Many: Account (1,1):(0,n) Beneficiaries  
      // ---------------------------------------------------
      modelBuilder.Entity<Beneficiary>(e => {
         e.HasOne<Account>(account => null)              // navigation property not modelled
            .WithMany(account => account.Beneficiaries)  // Account     --> Beneficiary [0..*]
            .HasForeignKey(beneficiary => beneficiary.AccountId) // Fk in Beneficiary
            .HasPrincipalKey(account => account.Id)              // Pk in Account
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
         //e.Navigation(beneficiary => beneficiary.Account); // no Navigation property
      });

      //
      // One-to-Many: Account (1,1): (0,n) Transfers 
      // ----------------------------------------------
      modelBuilder.Entity<Transfer>(e => {
         e.HasOne(transfer => transfer.Account)          // Transfer --> Account [1]
            .WithMany(account => account.Transfers)      // Account --> Transfer [0..*]
            .HasForeignKey(transfer => transfer.AccountId) // Fk in Transfer
            .HasPrincipalKey(account => account.Id)        // Pk in Account
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);
         e.Navigation(transfer => transfer.Account);     // Navigation property
      });
      
      // Account [1] <--> [0..*] Transactions One-To-Many
      // ------------------------------------------------
      modelBuilder.Entity<Transaction>(e => {
         e.HasOne(transaction => transaction.Account) // Transaction --> Account [1]
            .WithMany(account => account.Transactions)
            .HasForeignKey(transaction => transaction.AccountId)
            .HasPrincipalKey(account => account.Id)
            .IsRequired(false);
         e.Navigation(transaction => transaction.Account);
      });
      //
            // Transfer [--] <--> [0..1] Beneficiary
            // ------------------------------------
            modelBuilder.Entity<Transfer>(e => {
               e.HasOne(transfer => transfer.Beneficiary) // Transfer    --> Beneficiary [0..1]
                  .WithMany()                             // Beneficiary --> Transfer [--]
                  .HasForeignKey(transfer => transfer.BeneficiaryId) // Fk in Transfer
                  .HasPrincipalKey(beneficiary => beneficiary.Id)    // Pk in Beneficiary
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.NoAction);
               e.Navigation(transfer => transfer.Beneficiary);
            });
            //
            // Transaction [0..*] <--> [0..1] Transfer
            // ---------------------------------------
            modelBuilder.Entity<Transaction>(e => {
               e.HasOne(transaction => transaction.Transfer)
                  .WithMany(transfer => transfer.Transactions)
                  .HasForeignKey(transaction => transaction.TransferId)
                  .HasPrincipalKey(transfer => transfer.Id)
                  .IsRequired(false)
                  .HasPrincipalKey(transfer => transfer.Id)
                  .OnDelete(DeleteBehavior.NoAction);
               e.Navigation(transaction => transaction.Transfer);
            });
  
   }
   */
   
   public async Task<bool> SaveAllChangesAsync(
      string? text = null,
      CancellationToken ctToken = default
   ) {
      // log repositories before transfer to the database
      _logger?.LogInformation("\n{view}", ChangeTracker.DebugView.LongView);

      // save all changes to the database, returns the number of rows affected
      var result = await SaveChangesAsync(ctToken);

      // log repositories after transfer to the database
      _logger?.LogInformation("SaveChanges {result}", result);
      _logger?.LogInformation("\n{view}", ChangeTracker.DebugView.LongView);
      return result > 0;
   }

   public void ClearChangeTracker() =>
      ChangeTracker.Clear();

   public void LogChangeTracker(string text) =>
      _logger?.LogInformation("{text}\n{change}", text, ChangeTracker.DebugView.LongView);

   public static (string useDatabase, string dataSource) EvalDatabaseConfiguration(
      IConfiguration configuration
   ) {
      // appsettings      
      //"LocalFolder": "Webtech/Orms",
      //"UseDatabase": "Sqlite",
      // "ConnectionStrings": {
      //   "Sqlite": "Orm01"
      //},
      var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

      var localFolder = configuration.GetSection("LocalFolder").Value ??
         throw new Exception("LocalFolder is not available");
      localFolder.Split('/').ToList().ForEach(folder => { path = Path.Combine(path, folder); });
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);

      // read active database configuration from appsettings.json
      var useDatabase = configuration.GetSection("UseDatabase").Value ??
         throw new Exception("UseDatabase is not available");

      // read connection string from appsettings.json
      var connectionString = configuration.GetSection("ConnectionStrings")[useDatabase]
         ?? throw new Exception("ConnectionStrings is not available");

      switch (useDatabase) {
         case "LocalDb":
            var dbFile = $"{Path.Combine(path, connectionString)}.mdf";
            var dataSourceLocalDb =
               $"Data Source = (LocalDB)\\MSSQLLocalDB; " +
               $"Initial Catalog = {connectionString}; Integrated Security = True; " +
               $"AttachDbFileName = {dbFile};";
            Console.WriteLine($"....: EvalDatabaseConfiguration: LocalDb {dataSourceLocalDb}");
            Console.WriteLine();
            return (useDatabase, dataSourceLocalDb);
         case "SqlServer":
            return (useDatabase, connectionString);
         case "Sqlite":
            var dataSourceSqlite =
               "Data Source=" + Path.Combine(path, connectionString) + ".db";
            Console.WriteLine($"....: EvalDatabaseConfiguration: Sqlite {dataSourceSqlite}");
            return (useDatabase, dataSourceSqlite);
         case "SqliteInMemory":
            var dataSourceSqliteInMemory = "Data Source=:memory:";
            Console.WriteLine($"....: EvalDatabaseConfiguration: SqliteInMemory {dataSourceSqliteInMemory}");
            return (useDatabase, dataSourceSqliteInMemory);
         default:
            throw new Exception("appsettings.json Problems with database configuration");
      }
   }
}