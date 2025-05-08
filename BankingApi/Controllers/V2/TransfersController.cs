 using Asp.Versioning;
 using BankingApi.Core;
 using BankingApi.Core.DomainModel.Entities;
 using BankingApi.Core.Dto;
 using BankingApi.Core.Mapping;
 using Microsoft.AspNetCore.Mvc;
 namespace BankingApi.Controllers.V2;

 [Route("banking/v{version:apiVersion}")]
 [ApiVersion("2.0")]

 [ApiController]
 [Consumes("application/json")] //default
 [Produces("application/json")] //default

 public class TransfersController(
    IUseCasesTransfer useCasesTransfer,
    IAccountsRepository accountsRepository,
    ITransfersRepository transfersRepository
 ) : ControllerBase {
    
     [HttpGet("accounts/{accountId:guid}/transfers")]
     [EndpointSummary("Get transfers of an account by accountId")]
     [ProducesResponseType(StatusCodes.Status200OK)]
     [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
     public async Task<ActionResult<IEnumerable<TransferDto>>> GetByAccountIdAsync(
        [FromRoute] Guid accountId,
        CancellationToken ctToken = default
     ){
        var account = await accountsRepository.FindByIdAsync(accountId, ctToken);
        if(account == null)
           return BadRequest("Bad request: accountId does not exist.");
     
        var transfers =
           await transfersRepository.SelectByAccountIdAsync(accountId, ctToken);
        
        return Ok(transfers.Select(transfer => transfer.ToTransferDto()));
     }

     [HttpGet("transfers/{id:guid}")]
     [EndpointSummary("Get transfer by id")]
     [ProducesResponseType(StatusCodes.Status200OK)]
     [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
     public async Task<ActionResult<TransferDto>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken ctToken = default
     ){
        return await transfersRepository.FindByIdAsync(id,ctToken) switch {
           { } transfer => Ok(transfer.ToTransferDto()),
           null => NotFound("Transfer with given id not found.")
        };
     }

    [HttpPost("accounts/{accountId:guid}/transfers")]
    [EndpointSummary("Send Money: create a transfer with two transactions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<TransferDto?>> SendMoneyAsync(
       [FromRoute] Guid accountId,
       [FromBody] TransferDto transferDto,
       CancellationToken ctToken = default
    ){
       var transfer = transferDto.ToTransfer();
       
       // send money, i.e. create transfer and two transactions
       var resultData = 
          await useCasesTransfer.SendMoneyAsync(accountId, transfer, ctToken);
       
       // return errors
       if (resultData is Error<Transfer> && resultData.Status is not null) {
          return resultData.Status switch {
             400 => BadRequest(resultData.Message),
             404 => NotFound(resultData.Message),
             409 => Conflict(resultData.Message),
             _ => StatusCode(500, resultData.Message)
          };
       }
       
       // return an absolute URL as location 
       var url = "";
       if (Request != null) url = Request?.Scheme + "://" + Request?.Host
          + Request?.Path.ToString() +$"/{transfer.Id}";
       else url = $"http://localhost:5100/banking/v2/transfers/{transfer.Id}";
      
       var uri = new Uri(url, UriKind.Absolute);
       return Created(uri, transfer.ToTransferDto());
    }

    // Create a reverse transfer and the two transactions (debit/credit)
    [HttpPost("accounts/{accountId:guid}/transfers/reverse/{originalTransferId:guid}")]
    [EndpointSummary("Reverse Money: create a transfer with two transactions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TransferDto?>> ReverseMoneyAsync(
       [FromRoute] Guid accountId,
       [FromRoute] Guid originalTransferId,
       [FromBody] TransferDto reverseTransferDto,
       CancellationToken ctToken = default
    ){
       var originalTransfer = 
          await transfersRepository.FindByIdAsync(originalTransferId, ctToken);
       if (originalTransfer == null)
          return NotFound("Original transfer not found.");
       
       var reverseTransfer = new Transfer(
          id : Guid.NewGuid(),
          date : DateTime.UtcNow,
          description: "Reverse transfer",
          amount: -originalTransfer.Amount,
          accountId: originalTransfer.AccountId,
          beneficiaryId: originalTransfer.BeneficiaryId
       );
       reverseTransfer.SetBeneficiary(originalTransfer.Beneficiary);
       reverseTransfer.SetAccount(originalTransfer.Account);
       
       // reverse money, i.e. create reverse transfer and two transactions
       var resultData = 
          await useCasesTransfer.ReverseMoneyAsync(originalTransferId, reverseTransfer, ctToken);

       // return errors
       if (resultData is Error<Transfer> && resultData.Status is not null) {
          return resultData.Status switch {
             400 => BadRequest(resultData.Message),
             404 => NotFound(resultData.Message),
             409 => Conflict(resultData.Message),
             _ => StatusCode(500, resultData.Message)
          };
       }
       
       // return an absolute URL as location 
       var url = "";
       if (Request != null) url = Request?.Scheme + "://" + Request?.Host
          + Request?.Path.ToString() +$"/{reverseTransferDto.Id}";
       else url = $"http://localhost:5100/banking/v2/transfers/{reverseTransferDto.Id}";
      
       var uri = new Uri(url, UriKind.Absolute);
       return Created(uri, reverseTransferDto );
    }
}

