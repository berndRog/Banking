using System;
namespace BankingApi.Core.Dto;

public record AccountDto(
   Guid Id,
   string Iban,
   decimal Balance,
   Guid OwnerId
); 
