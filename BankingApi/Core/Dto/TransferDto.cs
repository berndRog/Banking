using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dto;

public record TransferDto (
   Guid Id,
   DateTime Date,
   [MaxLength(200)]
   string Description,
   decimal Amount,
   Guid AccountId,
   Guid? BeneficiaryId
);