namespace BankingApiTest.Core.Misc;

public class DateTimeExtensionUt {

   /*
   [Fact]
   public void NowToUniversalTimeUt() {
      // Arrange
      var localNow = DateTime.Now;
      // Act
      var utcNow = localNow.ToUniversalTime();
      // Assert
      localNow.Kind.Should().Be(DateTimeKind.Local);
      utcNow.Kind.Should().Be(DateTimeKind.Utc);
      var offset = TimeZoneInfo.Local.GetUtcOffset(localNow);
      utcNow.Should().Be(localNow - offset);
   }

   [Fact]
   public void UtcNowToLocalTimeUt() {
      // Arrange
      var utcNow = DateTime.UtcNow;
      // Act
      var localNow = utcNow.ToLocalTime();
      // Assert
      localNow.Kind.Should().Be(DateTimeKind.Local);
      utcNow.Kind.Should().Be(DateTimeKind.Utc);
      var offset = TimeZoneInfo.Local.GetUtcOffset(localNow);
      localNow.Should().Be(utcNow + offset);
   }
   
   [Fact]
   public void ToZonedDateTimeUt() {
      // Arrange
      // Source: New York
      // Windows TimeZoneId                              IANA TimeZoneId
      var sourceZoneId = Utils.ConvertToWindowsTimezone("America/New_York");
      var sourceDateTime = new DateTime(2024, 5, 19, 22, 30, 9, DateTimeKind.Unspecified);
      var sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById(sourceZoneId);
      var sourceOffset = sourceTimeZone.GetUtcOffset(sourceDateTime);
      var sourceIsDaylightSaving = sourceTimeZone.IsDaylightSavingTime(sourceDateTime); 
      var sourceTzName = sourceTimeZone.StandardName;
      if (sourceIsDaylightSaving) sourceTzName = sourceTimeZone.DaylightName;
      
      // ZonedDateTime
      DateTimeOffset sourceDateTimeOffset = new DateTimeOffset(sourceDateTime, sourceOffset);
      
      // Target: Berlin
      // Windows TimeZoneId                              IANA TimeZoneId
      var targetZoneId = Utils.ConvertToWindowsTimezone("Europe/Berlin");
      var targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(targetZoneId);
      var targetOffset = targetTimeZone.GetUtcOffset(sourceDateTimeOffset);
      var targetDateTime = sourceDateTimeOffset.ToOffset(targetOffset);
      var targetIsDaylightSaving = targetTimeZone.IsDaylightSavingTime(targetDateTime); 
      var targetTzName = targetTimeZone.StandardName;
      if (targetIsDaylightSaving) targetTzName = targetTimeZone.DaylightName;
      
      var now      = new DateTime(2024, 5, 19, 12, 30, 9, DateTimeKind.Unspecified);     
      var localNow = new DateTime(2024, 5, 19, 12, 30, 9, DateTimeKind.Local);     
      var utcNow   = new DateTime(2024, 5, 19, 12, 30, 9, DateTimeKind.Utc);     
      var localNow2 = now.ToLocalTime();      // DateTimeKind = DateTimeKind.Local   
      var utcNow2   = now.ToUniversalTime();  // DateTimeKind = DateTimeKind.Utc
      
      var localNow3 = utcNow2.ToLocalTime();      // DateTimeKind = DateTimeKind.Local
      var utcNow3   = localNow2.ToUniversalTime();  // DateTimeKind = DateTimeKind.Utc
      
      // Act
      //Utils.ToLocalDateTime(localNow, TimeZoneInfo.Local);
      // Assert
      //      localNow.Kind.Should().Be(DateTimeKind.Local);
      //      utcNow.Kind.Should().Be(DateTimeKind.Utc);
      //      var offset = TimeZoneInfo.Local.GetUtcOffset(localNow);
      //      localNow.Should().Be(utcNow + offset);
   }
   
   */
}