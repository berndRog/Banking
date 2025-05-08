using System;
using BankingApi;
using BankingApi.Core;
using BankingApi.Data;
using BankingApiTest.Di;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace BankingApiTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public abstract class BaseRepositoryUt {
   
   protected readonly IOwnersRepository _ownersRepository;
   protected readonly IAccountsRepository _accountsRepository;
   protected readonly IBeneficiariesRepository _beneficiariesRepository;
   protected readonly ITransfersRepository _transfersRepository;
   protected readonly ITransactionsRepository _transactionsRepository;
   protected readonly IDataContext _dataContext;
   protected readonly IUseCasesTransfer _useCasesTransfer;
   protected readonly ArrangeTest _arrangeTest;
   protected readonly Seed _seed;

   protected BaseRepositoryUt() {
      
      // Test DI-Container
      IServiceCollection services = new ServiceCollection();
      // Add Core, UseCases, ...
      services.AddCore();
      // Add Repositories, Test Databases, ...
      var (useDatabase, dataSource) = services.AddDataTest();
      // Service Locator Pattern
      var serviceProvider = services.BuildServiceProvider()
         ?? throw new Exception("Failed to create an instance of ServiceProvider");

      //-- Service Locator    
      var dbContext = serviceProvider.GetRequiredService<DataContext>()
         ?? throw new Exception("Failed to create DbContext");
      // In-Memory
      if (useDatabase == "SqliteInMemory") {
         dbContext.Database.OpenConnection();
      } else if (useDatabase == "Sqlite") {
         dbContext.Database.EnsureDeleted();
         // Workaround  SQLite I/O Errors
         SqliteConnection connection = new SqliteConnection(dataSource);
         connection.Open();
         using var command = connection.CreateCommand();
         // Switch the journal mode from WAL to DELETE.
         command.CommandText = "PRAGMA journal_mode = DELETE;";
         var result = command.ExecuteScalar();
         Console.WriteLine("Current journal mode: " + result);
         connection.Close();
      } else {
         dbContext.Database.EnsureDeleted();
      }
      dbContext.Database.EnsureCreated();

      _dataContext = serviceProvider.GetRequiredService<IDataContext>()
         ?? throw new Exception("Failed to create an instance of IDataContext");

      _ownersRepository = serviceProvider.GetRequiredService<IOwnersRepository>()
         ?? throw new Exception("Failed create an instance of IOwnersRepository");
      _accountsRepository = serviceProvider.GetRequiredService<IAccountsRepository>()
         ?? throw new Exception("Failed create an instance of IAccountsRepository");
      _beneficiariesRepository = serviceProvider.GetRequiredService<IBeneficiariesRepository>()
         ?? throw new Exception("Failed create an instance of IBeneficiariesRepository");
      _transactionsRepository = serviceProvider.GetRequiredService<ITransactionsRepository>()
         ?? throw new Exception("Failed create an instance of ITransactionsRepository");
      _transfersRepository = serviceProvider.GetRequiredService<ITransfersRepository>()
         ?? throw new Exception("Failed create an instance of ITransfersRepository");
      
      _useCasesTransfer = serviceProvider.GetRequiredService<IUseCasesTransfer>()
         ?? throw new Exception("Failed create an instance of IUseCasesTransfer");

      
      _arrangeTest = serviceProvider.GetRequiredService<ArrangeTest>()
         ?? throw new Exception("Failed create an instance of ArrangeTest");

      _seed = new Seed();
   }
}