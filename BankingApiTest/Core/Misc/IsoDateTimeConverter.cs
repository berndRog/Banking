namespace BankingApiTest.Core.Misc;

public class IsoDateTimeConverterUt {

   /*
   [Fact]
   public void ConvertIsoHHmmssUt() {
      // Arrange
      var iso8601 = "2022-04-01T12:34:56Z";
      // interpret as UTC
      var dt = new DateTime(2022, 4, 1, 12, 34, 56);
      var expected = dt.AsUniversalTime();
      var converter = new IsoDateTimeConverter();
      
      // Act
      var actual = converter.ParseIsoToUtc(iso8601);

      // Assert
      actual.Should().NotBeNull();
      actual.Should().Be(expected);
      actual?.Kind.Should().Be(DateTimeKind.Utc);
   }

   [Fact]
   public void ConvertIsoHHmmssfffUt() {
      // Arrange
      var iso8601 = "2022-04-01T12:34:56.789Z";
      // interpret as UTC
      var dt = new DateTime(2022, 4, 1, 12, 34, 56, 789);
      var expected = dt.AsUniversalTime();
      var converter = new IsoDateTimeConverter();
      
      // Act
      var actual = converter.ParseIsoToUtc(iso8601);

      // Assert
      actual.Should().NotBeNull();
      actual.Should().Be(expected);
      actual?.Kind.Should().Be(DateTimeKind.Utc);
   }

   [Fact]
   public void ConvertIsoHHmmssffffffUt() {
      // Arrange
      var iso8601 = "2022-04-01T12:34:56.78950Z";
      // interpret as UTC
      var dt = new DateTime(2022, 4, 1, 12, 34, 56, 789).AddTicks(500L * 10);
      var expected = dt.AsUniversalTime();
      var converter = new IsoDateTimeConverter();
      
      // Act
      var actual = converter.ParseIsoToUtc(iso8601);

      // Assert
      actual.Should().NotBeNull();
      actual.Should().Be(expected);
      actual?.Kind.Should().Be(DateTimeKind.Utc);
   }
/*   
   [Fact]
   public void ConvertIsoTzp1Ut() {
      // Arrange
      var iso8601 = "2022-04-01T12:34:56+1:00";
      // interpret as UTC
      var dt = new DateTime(2022, 4, 1, 12, 34, 56);
      
      var expected = dt.AsUniversalTime();
      var converter = new IsoDateTimeConverter();
      
      // Act
      var actual = converter.ParseIsoToUtc(iso8601);

      // Assert
      actual.Should().NotBeNull();
      actual.Should().Be(expected);
      actual?.Kind.Should().Be(DateTimeKind.Utc);
   }
   
   [Fact]
   public void Iso8601SerializeUt() {
      // Arrange
                               // yyyy  MM  dd  HH  mm  ss  fff
      var dateTime = new DateTime(2023, 12, 31, 23, 40, 50, 100).ToUniversalTime();
      var isoDateTimeConverter = new IsoDateTimeConverter();
      const string expected = "2023-12-31T23:40.100Z";

      // Act
      var actual = JsonSerializer.Serialize(
         dateTime,
         new JsonSerializerOptions {Converters = {isoDateTimeConverter}}
      );
      
      // Assert
      actual.Should().Be(expected);
   }
 
   
   [Fact]
   public void Iso8601DeserializeUt() {
      // Arrange
      const string iso = "{ \"datetime\" = \"2023-12-31T23:40.100Z\" }";
      //                          yyyy  MM  dd  HH  mm  ss  fff
      var expected = new DateTime(2023, 12, 31, 23, 40, 50, 100).ToUniversalTime();
      var isoDateTimeConverter = new IsoDateTimeConverter();
      
      // Act
      var actual = JsonSerializer.Deserialize<DateTime>(
         iso,
         new JsonSerializerOptions {Converters = {isoDateTimeConverter}}
      );
      
      // Assert
      actual.Should().Be(expected);
   }
   */
}