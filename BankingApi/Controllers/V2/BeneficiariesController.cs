using System.Net.Mime;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using BankingApi.Core.Misc;
using Microsoft.AspNetCore.Mvc;
namespace BankingApi.Controllers.V2;
[Route("banking/v{version:apiVersion}")]
[ApiVersion("2.0")]

[ApiController]
[Consumes("application/json")] //default
[Produces("application/json")] //default

public class BeneficiariesController(
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   IDataContext dataContext
): ControllerBase {
   
   [HttpGet("beneficiaries")]
   [EndpointSummary("Get all beneficiaries")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<AccountDto>> GetAllAsync(
      CancellationToken ctToken = default
   ) {
      var beneficiaries = await beneficiariesRepository.SelectAsync(false, ctToken);
      return Ok(beneficiaries.Select(b => b.ToBeneficiaryDto()));
   }


   [HttpGet("accounts/{accountId:guid}/beneficiaries")]
   [EndpointSummary("Get beneficiaries of an account by accountId")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<IEnumerable<BeneficiaryDto>>> GetByAccountIdAsync(
      [FromRoute] Guid accountId,
      CancellationToken ctToken = default
   ){
      var account = await accountsRepository.FindByIdAsync(accountId, ctToken);
      if(account == null)
         return BadRequest("Bad request: accountId does not exist.");

      var beneficiaries =
         await beneficiariesRepository.SelectByAccountIdAsync(accountId, ctToken);

      return Ok(beneficiaries.Select(beneficiary => beneficiary.ToBeneficiaryDto()) );
   }
   
   [HttpGet("beneficiaries/{id:guid}")]
   [EndpointSummary("Get a beneficiary by Id")]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<BeneficiaryDto>> GetByIdAsync(
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ){
      return await beneficiariesRepository.FindByIdAsync(id, ctToken) switch {
         { } beneficiary => Ok(beneficiary.ToBeneficiaryDto()),
         null => NotFound("Beneficiary with given id not found.")
      };
   }

   [HttpGet("beneficiaries/name/{name}")]
   [EndpointSummary("Get beneficiaries name, SQL like %name%")]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<IEnumerable<BeneficiaryDto>>> GetByNameAsync(
      [FromRoute] string name,
      CancellationToken ctToken = default
   ){
      // Find beneficiaries by SQL like %name%
      var beneficiaries = await beneficiariesRepository.SelectByNameAsync(name, ctToken);

      // if owners are found return them
      if (beneficiaries.Any()) return Ok(beneficiaries.Select(beneficiary => beneficiary.ToBeneficiaryDto()));
      // else return not found
      return NotFound("Beneficiaries with given Name not found");
   }

   [HttpGet("beneficiaries/iban/{iban}")]
   [EndpointSummary("Get a beneficiary by Iban")]
   [ProducesResponseType(typeof(BeneficiaryDto), StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<IEnumerable<BeneficiaryDto>>> GetByIbanAsync(
      [FromRoute] string iban,
      CancellationToken ctToken = default
   ){
      // Find beneficiaries by SQL like %name%
      var beneficiary = await beneficiariesRepository.FindByAsync(b=> b.Iban == iban, ctToken);
      if(beneficiary == null)   
         return NotFound("Beneficiary with given Iban not found");
      
      return Ok(beneficiary.ToBeneficiaryDto());
   }
   
   [HttpPost("accounts/{accountDebitId:guid}/beneficiaries")]
   [EndpointSummary("Create a new beneficiary")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<BeneficiaryDto?>> CreateAsync(
      [FromRoute] Guid accountDebitId,
      [FromBody] BeneficiaryDto beneficiaryDto,
      CancellationToken ctToken = default
   ){
      // Trim Iban and Check if Iban is valid
      beneficiaryDto = beneficiaryDto with { Iban = Utils.CheckIban(beneficiaryDto.Iban) };
      
      // check if beneficiaryDto.Id is empty
      if (beneficiaryDto.Id == Guid.Empty)
         beneficiaryDto = beneficiaryDto with { Id = Guid.NewGuid() };
      // check if beneficiaryDto.Id already exists
      if (await beneficiariesRepository.FindByIdAsync(beneficiaryDto.Id, ctToken) != null)
         return BadRequest("Beneficiary with given id already exists.");
      
      // Credit Account
      var accountCredit = 
         await accountsRepository.FindByAsync(a => a.Iban == beneficiaryDto.Iban, ctToken);
      // check if account with given Iban exists
      if(accountCredit == null)
          return NotFound("Beneficiary: Credit account with given Iban not found");
      
      // Debit account
      var accountDebit = await accountsRepository.FindByIdAsync(accountDebitId, ctToken);
      if(accountDebit == null)
         return NotFound("Beneficiary: Debit accountId not found.");
      
      // Domain model
      var beneficiary = beneficiaryDto.ToBeneficiary();
      accountDebit.AddBeneficiary(beneficiary);

      // save to repository and write to database
      beneficiariesRepository.Add(beneficiary);
      await dataContext.SaveAllChangesAsync("Add Beneficiary", ctToken);

      // return an absolute URL as location 
      var url = "";
      if (Request != null) url = Request?.Scheme + "://" + Request?.Host
         + Request?.Path.ToString() +$"/{beneficiary.Id}";
      else url = $"http://localhost:5100/banking/v2/beneficiaries/{beneficiary.Id}";
     
      var uri = new Uri(url, UriKind.Absolute);
      return Created(uri, beneficiary.ToBeneficiaryDto());
   }
   
   // Delete a beneficiary by Id.
   [HttpDelete("accounts/{accountId:guid}/beneficiaries/{id:guid}")]
   [EndpointSummary("Delete a benfificary with given id and with a given accountId")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<IActionResult> DeleteAsync(
      [FromRoute] Guid accountId,
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ){
      // Debit account
      var account = await accountsRepository.FindByIdAsync(accountId,ctToken);
      if(account == null  || account.Id != accountId)
         return BadRequest("Bad request: accountId does not exist.");
      
      // check if beneficiary with given id exists
      var beneficiary = await beneficiariesRepository.FindByIdAsync(id, ctToken);
      if(beneficiary == null)
         return NotFound("DeleteBeneficiary: Beneficiary with given id not found.");

      if(beneficiary.AccountId != accountId)
         return BadRequest("Bad request: accountId does not match.");
      
      // Load all transfers linked with the beneficiary
      var transfers = 
         await transfersRepository.FilterByAsync(tf => tf.BeneficiaryId == id, ctToken);
      
      foreach(var transfer in transfers) {
         // delete fk, don't delete the transfer 
         transfer.SetBeneficiary(null);
         transfersRepository.Update(transfer);
      }
      
      // save to repository and write to database 
      beneficiariesRepository.Remove(beneficiary);
      await dataContext.SaveAllChangesAsync("Remove Benefificary",ctToken);

      return NoContent();
   }
}