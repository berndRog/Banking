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


   protected override void OnModelCreating(ModelBuilder modelBuilder) {
      
      base.OnModelCreating(modelBuilder);
      
      //
      // PROPERTY CONFIGURATION
      //
      modelBuilder.Entity<Owner>(e => {
         e.ToTable("Owners"); // tablename Owners
         e.HasKey(owner => owner.Id); // primary key
         e.Property(owner => owner.Id) // primary key has type Guid
            // Id is generated on Add, when not existing
            .ValueGeneratedOnAdd()
            .IsRequired(); 
         e.Property(owner => owner.Name)
            .IsRequired()
            .HasMaxLength(100);
         e.Property(e => e.Birthdate)
            .HasConversion(
               v => v, // convert to UTC before saving
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); // set as UTC when loading
         e.Property(owner => owner.Email)
            .IsRequired(false) // email is optional
            .HasMaxLength(200);
       });
      
      modelBuilder.Entity<Account>(e => {
         e.ToTable("Accounts");
         e.HasKey(account => account.Id);
         e.Property(account => account.Id)
            .IsRequired()
            // Id is generated on Add, when not existing
            .ValueGeneratedOnAdd();
         e.Property(account => account.Iban)
            .IsRequired()
            .HasMaxLength(22);
         e.Property(account => account.Balance)
            .IsRequired()
            .HasPrecision(18, 2); // decimal with 18 digits and 2 decimal places
         // navigation properties are defined later
      });
      
      modelBuilder.Entity<Beneficiary>(e => {
         e.ToTable("Beneficiaries");
         e.HasKey(beneficiary => beneficiary.Id);
         e.Property(beneficiary => beneficiary.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
         e.Property(beneficiary => beneficiary.Name)
            .IsRequired()
            .HasMaxLength(100);
         e.Property(beneficiary => beneficiary.Iban)
            .IsRequired()
            .HasMaxLength(22); 
         // navigation properties are defined later
      });

      modelBuilder.Entity<Transfer>(e => {
         e.ToTable("Transfers");
         e.HasKey(transfer => transfer.Id);
         e.Property(transfer => transfer.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
         e.Property(e => e.Date)
            .HasConversion(
               v => v, 
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
         e.Property(transfer => transfer.Description)
            .IsRequired()
            .HasMaxLength(200);
         e.Property(transfer => transfer.Amount)
            .IsRequired()
            .HasPrecision(18, 2); // decimal with 18 digits and 2 decimal places
         // navigation properties are defined later
      });

      modelBuilder.Entity<Transaction>(e => {
         e.ToTable("Transactions"); 
         e.HasKey(transaction => transaction.Id); 
         e.Property(transaction => transaction.Id)
            .ValueGeneratedOnAdd()
            .IsRequired(); 
         e.Property(e => e.Date)
            .HasConversion(
               v => v, 
               v => DateTime.SpecifyKind(v, DateTimeKind.Utc)); 
         e.Property(transfer => transfer.Amount)
            .IsRequired()
            .HasPrecision(18, 2); // decimal with 18 digits and 2 decimal places
         // navigation properties are defined later
      });
      
      //
      // RELATIONS
      //
      // One-to-Many: Owner (1,1):(0:n] Account 
      // --------------------------------------
      modelBuilder.Entity<Account>(e => {
         e.HasOne(account => account.Owner)                       // Account --> Owner   (1,1)
            .WithMany(owner => owner.Accounts)                    // Owner   --> Account (0:n)
            .HasForeignKey(account => account.OwnerId)            // fk in Account
            .HasPrincipalKey(owner => owner.Id)                   // pk in Owner
            .OnDelete(DeleteBehavior.Cascade)                     // delete all accounts when owner is deleted
            .IsRequired();
         e.Navigation(account => account.Owner);                  // navigation property
      });
      //
      // One-To-Many: Account (1,1):(0,n) Beneficiaries  
      // ---------------------------------------------------
      modelBuilder.Entity<Beneficiary>(e => {
         e.HasOne<Account>()                                      // Beneficiary --> Account not modelled
            .WithMany(account => account.Beneficiaries)           // Account     --> Beneficiary (0:n)
            .HasForeignKey(beneficiary => beneficiary.AccountId)  // fk in Beneficiary
            .HasPrincipalKey(account => account.Id)               // pk in Account
            .OnDelete(DeleteBehavior.Cascade)                     // delete all beneficiaries when account is deleted
            .IsRequired();
         //e.Navigation(beneficiary => beneficiary.Account);      // no Navigation property
      });
      //
      // One-to-Many: Account (1,1): (0,n) Transfers 
      // ----------------------------------------------
      modelBuilder.Entity<Transfer>(e => {
         e.HasOne(transfer => transfer.Account)                   // Transfer --> Account (1,1)
            .WithMany(account => account.Transfers)               // Account --> Transfer (0:n)
            .HasForeignKey(transfer => transfer.AccountId)        // fk in Transfer
            .HasPrincipalKey(account => account.Id)               // pk in Account
            .OnDelete(DeleteBehavior.Cascade)                     // delete all transfers when account is deleted
            .IsRequired();
         e.Navigation(transfer => transfer.Account);              // Navigation property
      });
      //
      // One-to-Many: Account (1,1): (0,n) Transactions
      // ------------------------------------------------
      modelBuilder.Entity<Transaction>(e => {
         e.HasOne(transaction => transaction.Account)             // Transaction --> Account (1,1)
            .WithMany(account => account.Transactions)            // Account --> Transaction (0,n)
            .HasForeignKey(transaction => transaction.AccountId)  // fk in Transaction
            .HasPrincipalKey(account => account.Id)               // pk in Account
            .OnDelete(DeleteBehavior.Cascade)                     // delete all transactions when account is deleted
            .IsRequired();
         e.Navigation(transaction => transaction.Account);
      });
      //
      // ZeroOrOne-to-Conditional:  Beneficiary (0,1):(c) Transfer
      // ---------------------------------------------------------
      modelBuilder.Entity<Transfer>(e => {
         e.HasOne(transfer => transfer.Beneficiary)               // Transfer    --> Beneficiary (0,1)
            .WithMany()                                           // Beneficiary --> Transfer [c] not modelled
            .HasForeignKey(transfer => transfer.BeneficiaryId)    // fk in Transfer
            .HasPrincipalKey(beneficiary => beneficiary.Id)       // pk in Beneficiary
            .IsRequired(false)                                    // Beneficiary is optional
            .OnDelete(DeleteBehavior.NoAction);                   // no delete action when beneficiary is deleted
         e.Navigation(transfer => transfer.Beneficiary);
      });
      //
      // ZeroOrOne-To-Many: Transaction (0,2):(0,1) Transfer
      // ---------------------------------------------------
      modelBuilder.Entity<Transaction>(e => {
         e.HasOne(transaction => transaction.Transfer)            // Transaction --> Transfer (0,1)
            .WithMany(transfer => transfer.Transactions)          // Transfer    --> Transaction (0,n)
            .HasForeignKey(transaction => transaction.TransferId) // fk in Transaction
            .HasPrincipalKey(transfer => transfer.Id)             // pk in Transfer
            .IsRequired(false)                                    // Transfer is optional
            .OnDelete(DeleteBehavior.NoAction);                   // no delete action when transfer is deleted
         e.Navigation(transaction => transaction.Transfer);
      });

   }

   
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