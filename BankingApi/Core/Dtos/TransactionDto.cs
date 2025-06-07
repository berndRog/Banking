namespace BankingApi.Core.Dtos;

public record TransactionDto(
   Guid Id,
   DateTime Date,
   decimal Amount,
   Guid? AccountId,
   Guid? TransferId
);