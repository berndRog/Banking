using BankingApi.Core;
using BankingApi.Core.Misc;
using Microsoft.EntityFrameworkCore;
namespace BankingApi.Data.Repositories;

internal class ImagesRepository(
    DataContext dataContext
) : ABaseRepository<Image,Guid>(dataContext), IImagesRepository {
    // IMAGES
    public async Task<bool> ImageExistsAsync(Guid id) =>
        await _dataContext.Images
            .AnyAsync(i => i.Id == id);

    public async Task<Image?> GetImageAsync(Guid id) =>
        await _dataContext.Images
            .FirstOrDefaultAsync(i => i.Id == id);
    
    // IMAGE FILES
    public async Task<Image?> GetImageByUriPathAsync(string uriPath) =>
        await _dataContext.Images
            .FirstOrDefaultAsync(i => i.UrlString == uriPath);
    
    public async Task<(byte[], string, string)> LoadImageFile(
        string filePath, 
        string contentType
    ) {
        var readAllBytesAsync = await File.ReadAllBytesAsync(filePath);
        return (readAllBytesAsync, contentType, Path.GetFileName(filePath));
    }
    
    public async Task<string?> StoreImageFile(
        string path, 
        string mimeExtension,
        Stream stream
    ) {
        // Get the temporary folder, and combine a random file name with it
        var fileName = Path.GetRandomFileName();
        // To complicated if the file extension is obscured (how to load by coin?)
        fileName = Path.ChangeExtension(fileName, $"{mimeExtension}");
        
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var filePath = Path.Combine(path, fileName);

        await using FileStream targetStream = File.Create(filePath);
        await stream.CopyToAsync(targetStream);
        return fileName;
    }
}