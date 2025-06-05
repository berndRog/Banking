using System;
using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dto;

public record AccountDto(
   Guid Id,
   [MinLength(20), MaxLength(22)]
   string Iban,
   decimal Balance,
   Guid OwnerId
); 
