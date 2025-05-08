namespace BankingApi.Core.Dto;

public record TransferDto (
   Guid Id,
   DateTime Date,
   string Description,
   decimal Amount,
   Guid AccountId,
   Guid? BeneficiaryId
);