using System.ComponentModel.DataAnnotations;
namespace BankingApi.Core.Dto;

public record OwnerDto(
   Guid Id,
   [MinLength(2), MaxLength(100)]
   string Name,
   DateTime Birthdate,
   [EmailAddress]
   [MaxLength(200)]
   string? Email
);
