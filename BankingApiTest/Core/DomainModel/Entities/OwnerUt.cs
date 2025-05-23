using System;
using System.Collections.Generic;
using BankingApi.Core.DomainModel.Entities;
using Xunit;

namespace BankingApiTest.Core.DomainModel.Entities;
public class OwnerUt {

   private readonly Seed _seed = new Seed();

   #region without Account
   [Fact]
   public void Ctor1Ut() {
      // Arrange
      // Act
      var actual = new Owner();
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Owner>(actual);
   }

   [Fact]
   public void Ctor2Ut() {
      // Arrange
      // Act
      var actual = new Owner(
         id: _seed.Owner1.Id,
         name: _seed.Owner1.Name,
         birthdate:_seed.Owner1.Birthdate,
         email: _seed.Owner1.Email
      );
      // Assert
      Assert.NotNull(actual);
      Assert.IsType<Owner>(actual);
      Assert.Equal(_seed.Owner1.Id,  actual.Id); 
      Assert.Equal(_seed.Owner1.Name, actual.Name);
      Assert.Equal(_seed.Owner1.Birthdate, actual.Birthdate);
      Assert.Equal(_seed.Owner1.Email, actual.Email);
   }
   
   [Fact]
   public void GetterUt() {
      // Arrange
      var actual = _seed.Owner1;
      // Act
      var actualId = actual.Id;
      var actualName = actual.Name;
      var actualBirthdate = actual.Birthdate;
      var actualEmail = actual.Email;
      // Assert
      Assert.Equal(actualId, _seed.Owner1.Id);
      Assert.Equal(actualName, _seed.Owner1.Name);
      Assert.Equal(actualBirthdate, _seed.Owner1.Birthdate);
      Assert.Equal(actualEmail, _seed.Owner1.Email);
   }

   [Fact]
   public void UpdateUt() {
      // Arrange
      var actual = _seed.Owner1;
      var name = "Erna Meier";
      var email = "erna.meier@freenet.de";
      // Act
      actual.Update(name, email);
      // Assert
      Assert.Equal(name, actual.Name);
      Assert.Equal(email, actual.Email);
   }
   
   [Fact]
   public void UpdateUt_EmailOnly() {
      // Arrange
      var actual = _seed.Owner1;
      var email = "erna.meier@freenet.de";
      // Act
      actual.Update("", email);
      // Assert
      Assert.Equal(email, actual.Email);
   }
   [Fact]
   public void UpdateUt_NameOnly() {
      // Arrange
      var actual = _seed.Owner1;
      var name = "Erna Meier";
      // Act
      actual.Update(name, "");
      // Assert
      Assert.Equal(name, actual.Name);

   }
   #endregion

   #region with Accounts   
   [Fact]
   public void OwnerAddAccountUt() {
      // Arrange
      // Act
      _seed.Owner1.AddAccount(_seed.Account1);
      var expected = new List<Account> { _seed.Account1 };
      // Assert
      Assert.Single(_seed.Owner1.Accounts);
      Assert.Equivalent(expected, _seed.Owner1.Accounts);
   }
   #endregion
}