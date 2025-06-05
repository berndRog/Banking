using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dto;

public record BeneficiaryDto(
   Guid Id,
   [MinLength(2), MaxLength(100)] 
   string Name,
   [MinLength(20), MaxLength(22)]
   string Iban,
   Guid AccountId
); 