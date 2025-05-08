using System.Net.Mime;
using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
namespace BankingApi.Controllers;


/*
[ApiVersion("2.0")]
[Route("banking/v{version:apiVersion}")]
[ApiController]
public class ImagesController(
   IWebHostEnvironment webHostingEnvironment,
   IImagesRepository repository,
   IDataContext dataContext
) : ControllerBase {

   // Get an image by Id. 
   [HttpGet("images/{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(typeof(ImageDto), StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   public async Task<ActionResult<ImageDto>> GetById(
      [FromRoute] Guid id,
      CancellationToken ctToken = default
   ) {
      var image = await repository.FindByIdAsync(id, ctToken);
      if (image == null) return NotFound("Image with given id not found.");
      return Ok(image.ToImageDto());
   }

   /// <summary>
   /// Action for upload image file as FormFile (multipart/form-data)
   /// </summary>
   /// <remarks>
   /// Request to this action will not trigger any model binding or model validation,
   /// because this is a no-argument action
   /// </remarks>
   /// <returns>ActionResult{ImageDto}</returns>
   [HttpPost("images")]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<ImageDto>> Create(
      ImageDto imageDto
   ) {
      
      var image = imageDto.ToImage();      
      // save the image to repository and write to database
      repository.Add(image);
      await dataContext.SaveAllChangesAsync();

      // https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core
      // Request == null in unit tests
      var path = Request == null 
         ? $"/banking/v3/owners/{image.Id}" 
         : $"{Request.Path}/{image.Id}";
      var uri = new Uri(path, UriKind.Relative);
      return Created(uri: uri, value: image.ToImageDto());
   }
   
   /// <summary>
   /// Delete an image by Id, and the imageFile
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   /// <exception cref="IOException"></exception>
   [HttpDelete("images/{id:Guid}")]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult> Delete(
      [FromRoute] Guid id
   ) {
      var image = await repository.FindByIdAsync(id);
      if (image == null) return NotFound("Image with given id not found.");

      // delete imageFile if exists
      var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
      var fileName = image.UrlString!.Split('/').Last();
      var filePath = Path.Combine(path, fileName);
      try {
         if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
         else
            return NotFound("ImageFile not found on the server");
      }
      catch (IOException e) {
         return BadRequest(e.Message);
      }
      
      // delete the image from database   
      repository.Remove(image);
      await dataContext.SaveAllChangesAsync();
      
      return NoContent();
   }

   //
   // I M A G E   F I L E S 
   //
   // Get the imageFile by fileName
   [HttpGet("imageFiles/{fileName}")]
   [Produces(MediaTypeNames.Multipart.FormData)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   public async Task<IActionResult> DownloadFile(
      [FromRoute] string fileName
   ) {
      // get the complete uri path of the image file
      var uriPath = $"{Request.Scheme}://{Request.Host}{Request.Path}";
      if (uriPath.Split('/').Last() != fileName)
         return BadRequest("FileName and UriPath doesn't match");
      
      // get the image by uri path from database
      var image = await repository.GetImageByUriPathAsync(uriPath);
      if (image == null) return NotFound("Image with given uri path not found.");

      // get the local file path of the image file
      var filePath = Path.Combine(webHostingEnvironment.WebRootPath, "images", fileName);
      // load the file from local file system
      var (byteArray, contentType, fileDownloadName) =
         await repository.LoadImageFile(filePath, image!.ContentType);

      // return the imageFile
      return File(byteArray, contentType, fileDownloadName);
   }
   
   // Upload image file as FormFile (multipart/form-data)
   [HttpPost("owners/{ownerId:guid}/imageFiles")]
   [Consumes(MediaTypeNames.Multipart.FormData)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
   public async Task<ActionResult<ImageDto>> UploadFile(
      [FromRoute] Guid ownerId,
      CancellationToken ctToken = default
   ) {
      var request = HttpContext.Request;
      if (request == null) return BadRequest("No request.");

      // Check if request is multipart/form-data
      if (!request.HasFormContentType ||
          !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader1) ||
          string.IsNullOrEmpty(mediaTypeHeader1.Boundary.Value)) {
         return new UnsupportedMediaTypeResult();
      }
      
      // Get the FormFile collection from the request   
      IFormFileCollection files = request.Form.Files;
      if (files.Count == 0) return BadRequest("No file uploaded.");
      
      // Assuming single file upload
      IFormFile file = files[0];
      if (file.Length == 0) return BadRequest("Empty file uploaded.");
      // Get mime type from section header
      var mimeType = file.ContentType;
      if (!mimeType.StartsWith("image/")) return BadRequest("Invalid file type.");
      
      // Save the imageFile to the local file system
      var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
      var mimeExtension = mimeType switch {
         "image/jpeg" => ".jpg",
         "image/png" => ".png",
         "image/gif" => ".gif",
         _ => ".octet-stream"
      };
      var fileName = await repository.StoreImageFile(
         path, 
         mimeExtension, 
         file.OpenReadStream()
      );
      if (fileName == null) return BadRequest("File not saved.");

      var urlString = $"{request.Scheme}://{request.Host}{request.Path}/{fileName}";
      Image image = new(
         id: Guid.NewGuid(),
         urlString: urlString,
         contentType: mimeType!,
         updated: DateTime.UtcNow,
         ownerId: null
      );
      // save the image to repository and write to database
      repository.Add(image);
      await dataContext.SaveAllChangesAsync("Add Image", ctToken);

      var uri = new Uri(urlString, UriKind.Absolute);
      return Created(uri, image.ToImageDto());
   }
   
   // Update an existing imageFile
   [HttpPost("imageFiles/{fileName}")]
   [Consumes(MediaTypeNames.Multipart.FormData)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<ImageDto>> UpdateFile(
      [FromRoute] string fileName
   ) {
      var request = HttpContext.Request;
      if (request == null) return BadRequest("No request.");

      // Check if request is multipart/form-data
      if (!request.HasFormContentType ||
          !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader1) ||
          string.IsNullOrEmpty(mediaTypeHeader1.Boundary.Value)) {
         return new UnsupportedMediaTypeResult();
      }

      // Get the FormFile collection from the request   
      IFormFileCollection files = request.Form.Files;
      if (files.Count == 0) return BadRequest("No file uploaded.");
      // Assuming single file upload
      IFormFile file = files[0];
      if (file.Length == 0) return BadRequest("Empty file uploaded.");
      // Get mime type from section header
      var mimeType = file.ContentType;
      if (mimeType == null || !mimeType.StartsWith("image/")) return BadRequest("Invalid file type.");

      // Don't trust any file name, file extension, and file data from the request unless you trust them completely
      var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
      // Save the new file
      var mimeExtension = mimeType switch {
         "image/jpeg" => ".jpg",
         "image/png" => ".png",
         "image/gif" => ".gif",
         _ => ".octet-stream"
      };
      var newFileName = await repository.StoreImageFile(
         path, 
         mimeExtension, 
         file.OpenReadStream()
      );
      if (newFileName == null) return BadRequest("File not saved.");

      // get the complete encoded uri path of the image file
      var uriPath = $"{request.Scheme}://{request.Host}{request.Path}";
      // Retrieve the existing image from the repository
      var image = await repository.GetImageByUriPathAsync(uriPath);
      if (image == null) return NotFound($"Image not found.");
      // Delete the existin imageFile
      if (fileName == image.UrlString.Split('/').Last()) {
         var filePath = Path.Combine(path, uriPath.Split('/').Last());
         try {
            if (System.IO.File.Exists(filePath))
               System.IO.File.Delete(filePath);
            else
               throw new IOException("ImageFile not found on the server");
         }
         catch (IOException e) {
            return BadRequest(e.Message);
         }
      }

      // set the new uri path of the image file
      string[] parts = uriPath.Split('/');
      if (parts.Length < 2) return BadRequest("Invalid uri path.");
      parts[parts!.Length - 1] = newFileName;
      string remoteUriPath = String.Join("/", parts);
      
      // Update the existing image 
      image.Update(remoteUriPath, mimeType!, DateTime.UtcNow, null);
      repository.Update(image);
      await dataContext.SaveAllChangesAsync();

      var uri = new Uri(remoteUriPath, UriKind.Absolute);
      return Created(uri, image.ToImageDto());
   }
} 

/*
   /// <summary>
   /// Action for upload one large image file
   /// </summary>
   /// <remarks>
   /// Request to this action will not trigger any model binding or model validation,
   /// because this is a no-argument action
   /// </remarks>
   /// <returns></returns>
   [HttpPost("imageFiles")]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<ImageDto>> UploadFile() {
      var request = HttpContext.Request;
      if (request == null) return BadRequest("No request.");

      // Check if request is multipart/form-data
      if (!request.HasFormContentType ||
          !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
          string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value)) {
         return new UnsupportedMediaTypeResult();
      }
      // Get boundary from content-type header
      var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
      if (boundary == null) return BadRequest("Invalid boundary.");
      // Create multipart reader
      var reader = new MultipartReader(boundary, request.Body);
      var section = await reader.ReadNextSectionAsync();

      // This sample try to get the first file from request and save it
      if (section != null) {
         // Get mime type from section header
         var mimeType = section.ContentType;
         if (mimeType == null || !mimeType.StartsWith("image/")) return BadRequest("Invalid file type.");

         if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
             contentDisposition.DispositionType.Equals("form-data") &&
             !string.IsNullOrEmpty(contentDisposition.FileName.Value)) {
            // Don't trust any file name, file extension, and file data from the request unless you trust them completely
            var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
            // Save the imageFile to the local file system
            var fileName = await repository.StoreImageFile(path, section.Body);
            if (fileName == null) return BadRequest("File not saved.");

            // store the uri path and the contentType (Mime) of the imageFile in the image
            var imageUriPath = $"{request.Scheme}://{request.Host}{request.Path}/{fileName}";
            Image image = new() {
               UrlString = imageUriPath,
               ContentType = mimeType!,
               Updated = DateTime.UtcNow
               //  userId = AuthorizedUser.Id  for later use
            };

            // save the image to repository and write to database
            repository.Add(image);
            await dataContext.SaveAllChangesAsync();

            var uri = new Uri(imageUriPath, UriKind.Absolute);
            return Created(uri: uri, value: mapper.Map<ImageDto>(image));
         }
      }
      return BadRequest("No file data in the request.");
   }

   /// <summary>
   /// Action for upload a list of large image files
   /// </summary>
   /// <remarks>
   /// Request to this action will not trigger any model binding or model validation,
   /// because this is a no-argument action
   /// </remarks>
   /// <returns></returns>
   [HttpPost("imageFiles/multiple")]
   [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
   [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<string>> UploadFiles() {
      var request = HttpContext.Request;

      // Check if request is multipart/form-data
      if (!request.HasFormContentType ||
          !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
          string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value)) {
        return new UnsupportedMediaTypeResult();
      }
      // Get boundary from content-type header
      var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value;
      if (boundary == null) return BadRequest("Invalid boundary.");
      // Create multipart reader
      var reader = new MultipartReader(boundary, request.Body);
      var section = await reader.ReadNextSectionAsync();

      // Try to get the first file from request and save it
      while (section != null) {
         if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition) &&
             contentDisposition.DispositionType.Equals("form-data") &&
             !string.IsNullOrEmpty(contentDisposition.FileName.Value)) {
            // Don't trust any file name, file extension, and file data from the request unless you trust them completely
            var path = Path.Combine(webHostingEnvironment.WebRootPath, "images");
            var fileName = await repository.StoreImageFile(path, section.Body);
            if (fileName == null) return BadRequest("File not saved.");
            // The first image is saved only
            var uriPath = $"{Request.Path}/{fileName}";
            var uri = new Uri(uriPath, UriKind.Relative);
            return Created(uri, fileName);
         }
         section = await reader.ReadNextSectionAsync();
      }
      return BadRequest("No file data in the request.");
   }
*/