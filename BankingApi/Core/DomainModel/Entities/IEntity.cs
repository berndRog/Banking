namespace BankingApi.Core.DomainModel.Entities;
public interface IEntity<TId> {
   TId Id { get; init; }
}