using System.ComponentModel;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.Dtos;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
namespace BankingApi.Controllers.V2;
[ApiVersion("2.0")]
[Route("banking/v{version:apiVersion}")]

[ApiController]
[Consumes("application/json")] //default
[Produces("application/json")] //default
public class OwnersController(
   IOwnersRepository ownersRepository,
   IDataContext dataContext
): ControllerBase {
   
   [HttpGet("owners")]
   [EndpointSummary("Get all owners")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAllAsync(
      CancellationToken ctToken = default
   ) {
      var owners = await ownersRepository.SelectAsync(false, ctToken);
      return Ok(owners.Select(owner => owner.ToOwnerDto()));
   }
   
   [HttpGet("owners/{id:guid}")]
   [EndpointSummary("Get an owner by Id")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<OwnerDto?>> GetByIdAsync(
      [Description("Unique id of the owner to be found")]
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ) {
      return await ownersRepository.FindByIdAsync(id, ctToken) switch {
         // return owner as Dto
         { } owner => Ok(owner.ToOwnerDto()),
         // return not found
         null => NotFound("Owner with given Id not found")
      };
   }
   
   // GET http://localhost:5100/banking/v2/owners/name?name=xyz
   [HttpGet("owners/name")]
   [EndpointSummary("Get owners by name with SQL like %name%")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<IEnumerable<OwnerDto>>> GetByNameAsync(
      [Description("Name of the owner to be found with SQL like %name%")]
      [FromQuery] string name,
      CancellationToken ctToken = default
   ) {
      // Find owner by SQL like %name%
      var owners = await ownersRepository.SelectByNameAsync(name, ctToken);
      if (owners.Any()) return Ok(owners.Select(owner => owner.ToOwnerDto()));
      return NotFound("Owners with given Name not found");
   }

   [HttpPost("owners")]
   [EndpointSummary("Create a new owner")]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   public async Task<ActionResult<OwnerDto?>> CreateAsync(
      [Description("OwnerDto with the new owner's data")]
      [FromBody] OwnerDto ownerDto,
      CancellationToken ctToken = default
   ) {
      // check if ownerDto.Id is empty
      if (ownerDto.Id == Guid.Empty)
         ownerDto = ownerDto with { Id = Guid.NewGuid() };
      // check if Id already exists
      if (await ownersRepository.FindByIdAsync(ownerDto.Id,ctToken) != null)
         return BadRequest("CreateOwner: Owner with given id already exists.");
      
      // check if Name is too short or too long
      // check if Birthdate is too old or in the future
      // check if Email is too long or not valid
      
      // save to ownersRepository and write to database 
      var owner = ownerDto.ToOwner();
      ownersRepository.Add(owner);
      await dataContext.SaveAllChangesAsync("Add Owner",ctToken);
      
      // return an absolute URL as location 
      var url = "";
      if (Request != null) url = Request?.Scheme + "://" + Request?.Host
            + Request?.Path.ToString() +$"/{owner.Id}";
      else url = $"http://localhost:5100/banking/v2/owners/{owner.Id}";
     
      var uri = new Uri(url, UriKind.Absolute);
      return Created(uri, owner.ToOwnerDto());
   }
   
   [HttpPut("owners/{id:guid}")] 
   [EndpointSummary("Create an owner")]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, "application/problem+json")]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<ActionResult<OwnerDto?>> UpdateAsync(
      [Description("Unique id of the existing owner")]
      [FromRoute] Guid id,
      [Description("OwnerDto with the updated owner's data")]
      [FromBody]  OwnerDto updOwnerDto,
      CancellationToken ctToken = default
   ) {
 
      // check if Id in the route and body match
      if(id != updOwnerDto.Id) 
         return BadRequest("UpdateOwner: Id in the route and body do not match.");
      
      // check if owner with given Id exists
      var owner = await ownersRepository.FindByIdAsync(id, ctToken);
      if (owner == null)
         return NotFound("UpdateOwner: Owner with given id not found.");

      // Domain model: update owner (name and email only)
      owner.Update(updOwnerDto.Name, updOwnerDto.Email);
      // save to repository and write to database
      ownersRepository.Update(owner);
      await dataContext.SaveAllChangesAsync("Update Owner",ctToken);

      // return updated owner
      return Ok(owner.ToOwnerDto()); 
   }

   [HttpDelete("owners/{id:guid}")]
   [EndpointSummary("Delete an owner")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
   public async Task<IActionResult> DeleteAsync(
      [Description("Unique id of the existing owner")]
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ) {
      // check if owner with given Id exists
      var owner = await ownersRepository.FindByIdJoinAsync(id, true, ctToken);
      if (owner == null)
         return NotFound("DeleteOwner: Owner with given id not found.");
      
      ownersRepository.Remove(owner);
      await dataContext.SaveAllChangesAsync("Delete Owner",ctToken);
      
      // return no content
      return NoContent(); 
   }
}