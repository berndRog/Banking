using BankingApi.Core.DomainModel.Entities;
namespace BankingApi.Core.Misc;

public class Image: IEntity<Guid> {
    public Guid Id { get; init; } = Guid.NewGuid();
    public string UrlString { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public DateTime Updated { get; private set; } = DateTime.UtcNow;
    // navigation Property
    public Owner Owner { get; private set; } = null!;
    public Guid  OwnerId { get; private set; }
    
    public Image() {}
    public Image(Guid id, string urlString, string contentType, DateTime updated, Guid? ownerId) {
        Id = id;
        UrlString = urlString;
        ContentType = contentType;
        Updated = updated;
        if(ownerId.HasValue) OwnerId = ownerId.Value;
    }
    
    public void SetOwner(Owner owner) {
        Owner = owner;
        OwnerId = owner.Id;
    }
    
    public void Update(string urlString, string contentType, DateTime dateTime, Guid? ownerId) {
        UrlString = urlString;
        ContentType = contentType;
        Updated = dateTime;
        if(ownerId.HasValue) OwnerId = ownerId.Value;
    }
    
}