using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dtos;

public record TransferDto (
   Guid Id,
   DateTime Date,
   [MaxLength(200)]
   string Description,
   decimal Amount,
   Guid AccountId,
   Guid? BeneficiaryId
);