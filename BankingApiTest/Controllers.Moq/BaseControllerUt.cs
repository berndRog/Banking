using System;
using BankingApi.Controllers.V2;
using BankingApi.Core;
using BankingApi.Core.UseCases;
using BankingApiTest.Di;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BankingApiTest.Controllers.Moq;

public class BaseControllerUt {
   protected readonly Seed _seed;
   protected readonly Mock<IOwnersRepository> _mockOwnersRepository;
   protected readonly Mock<IAccountsRepository> _mockAccountsRepository;
   protected readonly Mock<IBeneficiariesRepository> _mockBeneficiariesRepository;
   protected readonly Mock<ITransfersRepository> _mockTransfersRepository;
   protected readonly Mock<ITransactionsRepository> _mockTransactionsRepository;
   protected readonly Mock<IDataContext> _mockDataContext;
   
   protected readonly IUseCasesTransfer _useCasesTransfer;
   
   protected readonly OwnersController _ownersController;
   protected readonly AccountsController _accountsController;
   protected readonly BeneficiariesController _beneficiariesController;
   protected readonly TransfersController _transfersController;
   protected readonly TransactionsController _transactionsController;
   
   protected BaseControllerUt() {
      var serviceCollection = new ServiceCollection();
      
      var serviceProvider = serviceCollection.BuildServiceProvider()
         ?? throw new Exception("Failed to build Serviceprovider");
      
      _seed = new Seed();
      
      // Mocking the repository and the data context
      _mockOwnersRepository = new Mock<IOwnersRepository>();
      _mockAccountsRepository = new Mock<IAccountsRepository>();
      _mockBeneficiariesRepository = new Mock<IBeneficiariesRepository>();
      _mockTransfersRepository = new Mock<ITransfersRepository>();
      _mockTransactionsRepository = new Mock<ITransactionsRepository>();
      
      _mockDataContext = new Mock<IDataContext>();
      
      
      // Mocking the use cases
      _useCasesTransfer = new UseCasesTransfer(
         _mockAccountsRepository.Object,
         _mockBeneficiariesRepository.Object,
         _mockTransfersRepository.Object,
         _mockTransactionsRepository.Object,
         _mockDataContext.Object
      );
      

      // Mocking the controller
      _ownersController = new OwnersController(
         _mockOwnersRepository.Object,
         _mockAccountsRepository.Object,
         _mockDataContext.Object
      );
      _accountsController = new AccountsController(
         _mockOwnersRepository.Object,
         _mockAccountsRepository.Object,
         _mockDataContext.Object
      );
      _beneficiariesController = new BeneficiariesController(
         _mockOwnersRepository.Object,
         _mockAccountsRepository.Object,
         _mockBeneficiariesRepository.Object,
         _mockTransfersRepository.Object,
         _mockDataContext.Object
      );
      _transfersController = new TransfersController(
         _useCasesTransfer,
         _mockAccountsRepository.Object,
         _mockTransfersRepository.Object
      );
      _transactionsController = new TransactionsController(
         _mockAccountsRepository.Object,
         //_mockTransfersRepository.Object,
         _mockTransactionsRepository.Object
      );

   }
}