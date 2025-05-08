using System;
using System.IO;
using System.Threading.Tasks;
using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Misc;
namespace BankingApi.Core;

public interface IImagesRepository: IBaseRepository<Image,Guid> {
    Task<Image?> GetImageAsync(Guid id);
    Task<Image?> GetImageByUriPathAsync(string uriPath);
    
    Task<(byte[], string, string)> LoadImageFile(string filePath, string contentType);
    Task<string?> StoreImageFile(string path, string mimeExtension, Stream stream);
    
}

