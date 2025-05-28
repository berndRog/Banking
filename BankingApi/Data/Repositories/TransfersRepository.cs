using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;
public class TransfersRepository(
   DataContext dataContext
): ABaseRepository<Transfer, Guid>(dataContext), ITransfersRepository {
}