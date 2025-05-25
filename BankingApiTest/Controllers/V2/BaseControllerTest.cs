using System;
using BankingApi.Controllers.V2;
using BankingApi.Core;
using BankingApi.Data;
using BankingApiTest.Di;
using BankingApiTest.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace BankingApiTest.Controllers.V2;
[Collection(nameof(SystemTestCollectionDefinition))]
public class BaseControllerTest {

   protected readonly OwnersController _ownersController;
   protected readonly AccountsController _accountsController;
   protected readonly BeneficiariesController _beneficiariesController;
   protected readonly TransfersController _transfersController;
   protected readonly TransactionsController _transactionsController;

   protected readonly IUseCasesTransfer _useCasesTransfer;
   
   protected readonly IOwnersRepository _ownersRepository;
   protected readonly IAccountsRepository _accountsRepository;
   protected readonly IBeneficiariesRepository _beneficiariesRepository;
   protected readonly ITransfersRepository _transfersRepository;
   protected readonly ITransactionsRepository _transactionsRepository;
   protected readonly IDataContext _dataContext;
   
   protected readonly ArrangeTest _arrangeTest;
   protected readonly Seed _seed;

   protected BaseControllerTest() {

      // Create test configuration
      var configuration = new ConfigurationBuilder()
         .AddJsonFile("appsettingsTest.json", optional: false)
         .Build();

      IServiceCollection serviceCollection = new ServiceCollection();

      // Add Repositories, Test Databases, ...
      var (useDatabase, dataSource) = serviceCollection.AddDataTest();
            

      var serviceProvider = serviceCollection.BuildServiceProvider()
         ?? throw new Exception("Failed to build Serviceprovider");

      var dbContext = serviceProvider.GetRequiredService<DataContext>()
         ?? throw new Exception("Failed to create an instance of DataContext");
      
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
         ?? throw new Exception("Failed to create an instance of IOwnersRepository");
      _accountsRepository = serviceProvider.GetRequiredService<IAccountsRepository>()
         ?? throw new Exception("Failed to create an instance of IAccountsRepository");
      _beneficiariesRepository = serviceProvider.GetRequiredService<IBeneficiariesRepository>()
         ?? throw new Exception("Failed to create an instance of IBeneficiariesRepository");
      _transfersRepository = serviceProvider.GetRequiredService<ITransfersRepository>()
         ?? throw new Exception("Failed to create an instance of ITransfersRepository");
      _transactionsRepository = serviceProvider.GetRequiredService<ITransactionsRepository>()
         ?? throw new Exception("Failed to create an instance of ITransactionsRepository");

      _useCasesTransfer = serviceProvider.GetRequiredService<IUseCasesTransfer>()
         ?? throw new Exception("Failed to create an instance of IUseCasesTransfer");

      _ownersController = new OwnersController(
         _ownersRepository,
         _accountsRepository,
         _dataContext
      );
      _accountsController = new AccountsController(
         _ownersRepository,
         _accountsRepository,
         _dataContext
      );
      _beneficiariesController = new BeneficiariesController(
         _ownersRepository,
         _accountsRepository,
         _beneficiariesRepository,
         _transfersRepository,
         _dataContext
      );
      _transfersController = new TransfersController(
         _useCasesTransfer,
         _accountsRepository,
         _transfersRepository
      );
      _transactionsController = new TransactionsController(
         _accountsRepository,
         _transactionsRepository
      );
      
      
      _arrangeTest = serviceProvider.GetRequiredService<ArrangeTest>()
         ?? throw new Exception("Failed create an instance of ArrangeTest");
      _seed = new Seed();
   }
}