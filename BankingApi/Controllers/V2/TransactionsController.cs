using System.ComponentModel;
using System.Globalization;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.Dto;
using BankingApi.Core.Dtos;
using BankingApi.Core.Mapping;
using BankingApi.Core.Misc;
using Microsoft.AspNetCore.Mvc;
namespace BankingApi.Controllers.V2;

[Route("banking/v{version:apiVersion}")]
[ApiVersion("2.0")]

[ApiController]
[Consumes("application/json")] //default
[Produces("application/json")] //default

public class TransactionsController(
   IAccountsRepository accountsRepository,
   ITransactionsRepository transactionsRepository
): ControllerBase {
   
   [HttpGet("transactions")]
   [EndpointSummary("Get all transactions")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllAsync(
      CancellationToken ctToken = default
   ) {
      var transactions = await transactionsRepository.SelectAsync(false, ctToken);
      transactions = transactions.OrderBy(t => t.Date);
      return Ok(transactions.Select(transaction => transaction.ToTransactionDto()));
   }
   
   [HttpGet("accounts/{accountId:guid}/transactions/filter")]
   [EndpointSummary("Get transactions for an account  by accountId and time intervall start to end")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   public async Task<ActionResult<IEnumerable<TransactionDto>>> GetByAccountIdAsync(
      [FromRoute] Guid accountId,
      [FromQuery] string start,
      [FromQuery] string end,
      CancellationToken ctToken = default
   ){
      var account = await accountsRepository.FindByIdAsync(accountId, ctToken);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");
      try {
         var (errorStart, dateTimeStart, errorMessageStart) = Utils.EvalDateTime(start);
         if(errorStart) return BadRequest(errorMessageStart);
      
         var (errorEnd, dateTimeEnd, errorMessageEnd) = Utils.EvalDateTime(end);
         if(errorEnd) return BadRequest(errorMessageEnd);

         var transactions =
            await transactionsRepository.FilterByAccountIdAsync(
               accountId,
               t => t.Date >= dateTimeStart && t.Date <= dateTimeEnd,
               ctToken
            );
         return Ok(transactions.Select(transaction => transaction.ToTransactionDto()));
      }
      catch {
         return BadRequest($"Transaction: Fehler Zeitstempel start:{start} end:{end}");
      }
   }

   [HttpGet("accounts/{accountId:guid}/transactions/listitems")]
   [EndpointSummary("Get transactionListItemDtos of an account by accountId and time intervall start to end")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   public async Task<ActionResult<IEnumerable<TransactionListItemDto>>> GetTransactionListItemsByAccountId(
      [Description("Unique accountId of the account for which we want to filter transactions")]
      [FromRoute] Guid accountId,
      [Description("Start date of the time intervall in ISO 8601 format")]
      [FromQuery] string start,
      [Description("End date of the time intervall in ISO 8601 format")]
      [FromQuery] string end,
      CancellationToken ctToken = default
   ){ 
      var account = await accountsRepository.FindByIdAsync(accountId, ctToken);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");

      try {
         var dateTimeStart =
            DateTime.Parse(start, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
         var dateTimeEnd = 
            DateTime.Parse(end, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
         
         var transactionListItemDtos =
            await transactionsRepository.FilterListItemsByAccountIdAsync(
               accountId,
               t => t.Date >= dateTimeStart && t.Date <= dateTimeEnd,
               ctToken
            );
         transactionListItemDtos = transactionListItemDtos
            .OrderBy(t => t.Date);
         return Ok(transactionListItemDtos);  // result already is in DTO format
      }
      catch(Exception ex) {
         return BadRequest($"Transaction: Error timestamp start:{start} end:{end}\n{ex.Message}");
      }
   }
   
   [HttpGet("transactions/{id:guid}")]
   [EndpointSummary("Get a transaction by Id")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<TransactionDto>> GetByIdAsync(
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ){
      var transaction = await transactionsRepository.FindByIdAsync(id, ctToken);
      if(transaction == null) return NotFound("Transaction with given id not found.");
      return Ok(transaction.ToTransactionDto());
   }
}