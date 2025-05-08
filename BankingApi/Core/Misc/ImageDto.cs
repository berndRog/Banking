using System;
namespace BankingApi.Core.Misc;

public record ImageDto(
   Guid Id,
   string UrlString,
   string ContentType,
   DateTime Updated,
   Guid OwnerId
);
