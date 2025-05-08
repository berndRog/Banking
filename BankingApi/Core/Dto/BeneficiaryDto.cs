using System;
namespace BankingApi.Core.Dto;

public record BeneficiaryDto(
   Guid Id,
   string Name,
   string Iban,
   Guid AccountId
);