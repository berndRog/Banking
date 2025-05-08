using System.Net.Mime;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
namespace BankingApi.Controllers.V2;
[Route("banking/v{version:apiVersion}")]
[ApiVersion("2.0")]

[ApiController]
[Consumes("application/json")] //default
[Produces("application/json")] //default

public class BeneficiariesController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IBeneficiariesRepository beneficiariesRepository,
   ITransfersRepository transfersRepository,
   IDataContext dataContext
): ControllerBase {
   
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

   [HttpPost("accounts/{accountId:guid}/beneficiaries")]
   [EndpointSummary("Create a new beneficiary")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<BeneficiaryDto?>> CreateAsync(
      [FromRoute] Guid accountId,
      [FromBody] BeneficiaryDto beneficiaryDto,
      CancellationToken ctToken = default
   ){
      // check if beneficiaryDto.Id is empty
      if (beneficiaryDto.Id == Guid.Empty)
         beneficiaryDto = beneficiaryDto with { Id = Guid.NewGuid() };
      // check if beneficiaryDto.Id already exists
      if (await beneficiariesRepository.FindByIdAsync(beneficiaryDto.Id, ctToken) != null)
         BadRequest("Beneficiary with given id already exists.");
      if(beneficiaryDto.AccountId != accountId)
         BadRequest("Bad request: accountId does not match.");
      
         
      var accountCredit = 
         await accountsRepository.FindByAsync(a => a.Iban == beneficiaryDto.Iban, ctToken);
      // check if account with given Iban exists
      if(accountCredit == null)
          NotFound("Beneficiary: Credit account with given Iban not found");
      
      // Get owners name by accout.OwnerId
      var owners = await ownersRepository.SelectByNameAsync(beneficiaryDto.Name, ctToken);
      var count = owners.Count();
      if(count == 0)
         NotFound("Beneficiary: Owner with given name not found");
      else if(count > 1)
         NotFound("Beneficiary: more then one owner found for the given name");
      var owner = owners.First();
      
      // Domain model
      var beneficiary = beneficiaryDto.ToBeneficiary();
      // Debit account
      var account = await accountsRepository.FindByIdAsync(beneficiary.AccountId, ctToken);
      if(account == null  || account.Id != accountId)
         return BadRequest("Bad request: accountId does not exist.");
      account.AddBeneficiary(beneficiary);

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

      
      // Load all transfers with the beneficiary to delete
      var transfers = await transfersRepository.SelectByBeneficiaryIdAsync(id, ctToken);
      foreach(var transfer in transfers) {
         transfer.SetBeneficiary(null);
         transfersRepository.Update(transfer);
      }
      
      // save to repository and write to database 
      beneficiariesRepository.Remove(beneficiary);
      await dataContext.SaveAllChangesAsync("Remove Benefificary",ctToken);

      return NoContent();
   }
}