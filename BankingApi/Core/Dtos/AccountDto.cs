using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dtos;

public record AccountDto(
   Guid Id,
   [IbanLength(20,22)]
   string Iban,
   decimal Balance,
   Guid OwnerId
); 
