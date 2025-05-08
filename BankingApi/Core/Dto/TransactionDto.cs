using System;
namespace BankingApi.Core.Dto;

/// <summary>
/// TransactionDto (Buchung)
/// </summary>
public record TransactionDto(
   Guid Id,
   DateTime Date,
   decimal Amount,
   Guid? AccountId,
   Guid? TransferId
);