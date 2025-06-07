using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dtos;

public record BeneficiaryDto(
   Guid Id,
   string Name,
   [IbanLength(20,22)]
   string Iban,
   Guid AccountId
); 