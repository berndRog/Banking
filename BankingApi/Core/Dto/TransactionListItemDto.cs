namespace BankingApi.Core.Dtos;

public record TransactionListItemDto(
   Guid Id,
   DateTime Date,
   decimal Amount,
   string Description, 
   string Name,  // receiver
   string Iban,       // receiver
   Guid AccountId,    // sender
   Guid TransferId    // sender/receiver
);
