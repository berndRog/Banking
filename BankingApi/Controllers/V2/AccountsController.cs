using System.ComponentModel;
using System.Net.Mime;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using BankingApi.Core.Misc;
using Microsoft.AspNetCore.Mvc;
namespace BankingApi.Controllers.V2;

[ApiVersion("2.0")]
[Route("banking/v{version:apiVersion}")]

[ApiController]
[Consumes("application/json")] //default
[Produces("application/json")] //default

public class AccountsController(
   IOwnersRepository ownersRepository,
   IAccountsRepository accountsRepository,
   IDataContext dataContext
) : ControllerBase {
   
   [HttpGet("accounts")]
   [EndpointSummary("Get all accounts")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<AccountDto>>> GetAllAsync(
      CancellationToken ctToken = default
   ) {
      var accounts = await accountsRepository.SelectAsync(false, ctToken);
      return Ok(accounts.Select(account => account.ToAccountDto()));
   }
   
   [HttpGet("owners/{ownerId:guid}/accounts")]
   [EndpointSummary("Get all accounts of a given ownerId")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<IEnumerable<AccountDto>>> GetByOwnerIdAsync(
      [Description("Unique ownerId of the existing owner")]
      [FromRoute] Guid ownerId,
      CancellationToken ctToken = default
   ) {
      var accounts = 
         await accountsRepository.SelectByOwnerIdAsync(ownerId, ctToken);
      return Ok(accounts.Select(account => account.ToAccountDto()));
   }

   [HttpGet("accounts/{id:guid}")]
   [EndpointSummary("Get an account by id")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<AccountDto?>> GetByIdAsync(
      [Description("Unique id of the account to be found")]
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ) {
      return await accountsRepository.FindByIdAsync(id, ctToken) switch {
         { } account => Ok(account.ToAccountDto()),
         null => NotFound("Account with given id not found")
      };
   }
   
   [HttpGet("accounts/iban/{iban}")]
   [EndpointSummary("Get an account by Iban")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<AccountDto?>> GetByIbanAsync(
      [Description("Unique iban of the account to be found")]
      [FromRoute] string iban,
      CancellationToken ctToken = default
   ) {
      var checkedIban = Utils.CheckIban(iban);
      if (checkedIban != iban.Replace(" ", "").ToUpper())
         return BadRequest("Iban is not valid.");
      
      return await accountsRepository.FindByAsync(a => a.Iban == checkedIban, ctToken) switch {
         { } account => Ok(account.ToAccountDto()),
         null => NotFound("Account with given Iban not found")
      };
   }

   
   [HttpPost("owners/{ownerId:guid}/accounts")]
   [EndpointSummary("Create a new account for a given ownerId")]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<AccountDto?>> CreateAsync(
      [Description("Unique ownerId of the existing owner")]
      [FromRoute] Guid ownerId,
      [Description("AccountDto with the new account's data")]
      [FromBody] AccountDto accountDto,
      CancellationToken ctToken = default
   ) {
      // check if accountDto.Id is empty
      if (accountDto.Id == Guid.Empty)
         accountDto = accountDto with { Id = Guid.NewGuid() };
      // check if account with given Id already exists
      if (await accountsRepository.FindByIdAsync(accountDto.Id, ctToken) != null)
         return BadRequest("Create Account: Account with given id already exists.");
      
      // get the owner for the account
      var owner = await ownersRepository.FindByIdAsync(ownerId, ctToken);
      if(owner == null)
         return BadRequest("Create Account: Owner for account doesn't exists.");
      
      // DomainModel
      var account = accountDto.ToAccount();
      owner.AddAccount(account);
      // save to repository and write to database 
      accountsRepository.Add(account);
      await dataContext.SaveAllChangesAsync("Add Account",ctToken);

      // return an absolute URL as location 
      var url = "";
      if (Request != null) url = Request?.Scheme + "://" + Request?.Host
         + Request?.Path.ToString() +$"/{owner.Id}";
      else url = $"http://localhost:5100/banking/v2/acconts/{account.Id}";
     
      var uri = new Uri(url, UriKind.Absolute);
      return Created(uri, account.ToAccountDto());
   }
   
   [HttpDelete("owners/{ownerId:guid}/accounts/{id:guid}")] 
   [EndpointSummary("Delete an account with given id and with a given ownerId")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<IActionResult> DeleteAsync(
      [Description("Unique ownerId of the existing owner")]
      [FromRoute] Guid ownerId, 
      [Description("Unique id of the existing account")]
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ) {
      // check if owner with given Id exists
      var owner = await ownersRepository.FindByIdAsync(ownerId, ctToken);
      if (owner == null)
         return NotFound("Delete Account: Owner not found.");

      var account = await accountsRepository.FindByIdJoinAsync(id, true, ctToken);
      if (account == null)
         return NotFound("Delete Account: Account not found.");
     
      if(account.OwnerId != owner.Id)
         return BadRequest("Delete Account: Owner and Account data do not match.");
      
      accountsRepository.Remove(account);
      await dataContext.SaveAllChangesAsync("Delete Account",ctToken);
      
      // return no content
      return NoContent(); 
   }
}