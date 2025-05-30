using BankingApi.Core.DomainModel.Entities;
using BankingApi.Core.Dto;
using BankingApi.Core.Misc;
namespace BankingApi.Core.Mapping;

public static class Mappings {
   // Entity Owner <-> DTO OwnerDto
   public static OwnerDto ToOwnerDto(this Owner owner) {
      return new OwnerDto(owner.Id, owner.Name, owner.Birthdate, owner.Email);
   }
   public static Owner ToOwner(this OwnerDto dto) {
      return new Owner(dto.Id, dto.Name, dto.Birthdate, dto.Email);
   }
   
   // Entity Account <-> DTO AccountDto
   public static AccountDto ToAccountDto(this Account entity) {
      return new AccountDto(entity.Id, entity.Iban, entity.Balance, entity.OwnerId);
   }
   public static Account ToAccount(this AccountDto dto) {
      return new Account(dto.Id, dto.Iban, dto.Balance, dto.OwnerId);
   }
   
   // Entity Beneficiary <-> DTO BeneficiaryDto
   public static BeneficiaryDto ToBeneficiaryDto(this Beneficiary entity) {
      return new BeneficiaryDto(entity.Id, entity.Name, entity.Iban, entity.AccountId);
   } 
   public static Beneficiary ToBeneficiary(this BeneficiaryDto dto) {
      return new Beneficiary(dto.Id, dto.Name, dto.Iban, dto.AccountId);
   }
   
   // Entity Transfer <-> DTO TransferDto
   public static TransferDto ToTransferDto(this Transfer entity) {
      return new TransferDto(entity.Id, entity.Date, entity.Description, entity.Amount, 
         entity.AccountId, entity.BeneficiaryId);
   }
   public static Transfer ToTransfer(this TransferDto dto) {
      return new Transfer(dto.Id, dto.Date, dto.Description, dto.Amount, 
         dto.AccountId, dto.BeneficiaryId);
   }
   
   // Entity transaction <-> DTO transactionDto
   public static TransactionDto ToTransactionDto(this Transaction entity) {
      return new TransactionDto(entity.Id, entity.Date, entity.Amount,
         entity.AccountId, entity.TransferId);
   }
   public static Transaction ToTransaction(this TransactionDto dto) {
      return new Transaction(dto.Id, dto.Date, dto.Amount,
         dto.AccountId, dto.TransferId);
   }
   
}