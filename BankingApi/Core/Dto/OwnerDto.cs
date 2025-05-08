using System;
namespace BankingApi.Core.Dto;

public record OwnerDto(
   Guid Id,
   string Name,
   DateTime Birthdate,
   string? Email
);
