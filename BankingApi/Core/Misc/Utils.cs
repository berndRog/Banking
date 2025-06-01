using System.Text;
using System.Globalization;
namespace BankingApi.Core.Misc;

public static class Utils {
   
   // Only accepts the 8-4-4-4-12 hyphen format
   public static bool IsValidGuidDFormat(string s)
      => Guid.TryParseExact(s, "D", out _);
   
   public static string CheckIban(string? iban) {
      //"DEkk BBBB BBBB CCCC CCCC CC"
      if (iban != null) {
         iban = iban.Replace(" ", "").ToUpper();
         if (iban.Length is >= 20 and <= 22) return iban;
      }
      
      throw new ArgumentException(
         "Iban is not valid. It must be 20-22 characters long and contain only digits and letters.");
      
      // if iban is not valid, create a new one
      var random = new Random();
      //     DEkk                           BBBB                    BBBB
      return "DE" + Digits2(random) + " " + Digits4(random) + " " + Digits4(random) + " " +
         // CCCC                    CCCC                   CC
         Digits4(random) + " " + Digits4(random) + " " + Digits2(random);
   }

   private static string Digits2(Random random) =>
      random.Next(1, 99).ToString("D2");

   private static string Digits4(Random random) =>
      random.Next(0, 1000).ToString("D4") + " ";

   public static string As8(this Guid guid) => guid.ToString()[..8];

/*
 * IBAN (International Bank Account Number) is a standardized format for bank account numbers.
 * It consists of a country code, check digits, and a Basic Bank Account Number (BBAN).
 * The length and structure vary by country.
 * 
 * This implementation validates IBANs for Germany (DE), Austria (AT), and Switzerland (CH).
 */
// public static class IbanValidator {
//    // Country → expected total length
//    private static readonly Dictionary<string, int> _lengths = new() {
//       ["DE"] = 22,
//       ["AT"] = 20,
//       ["CH"] = 21
//    };
//
//    public static bool IsValid(string iban) {
//       
//       if (string.IsNullOrWhiteSpace(iban)) return false;
//
//       // 1) Clean up
//       iban = iban
//          .Replace(" ", "")
//          .Replace("-", "")
//          .ToUpperInvariant();
//
//       // 2) Must be alphanumeric
//       foreach (char c in iban) {
//          if (!char.IsLetterOrDigit(c)) return false;
//       }
//
//       // 3) Country & length check
//       if (iban.Length < 4) return false;
//       var country = iban[..2];
//       if (!_lengths.TryGetValue(country, out var expectedLen)
//           || iban.Length != expectedLen) {
//          return false;
//       }
//
//       // 4) Rearrange: move first 4 chars to the end
//       var rearranged = iban[4..] + iban[..4];
//
//       // 5) Convert letters to digits (A=10 … Z=35) and compute mod-97
//       int remainder = 0;
//       foreach (char c in rearranged) {
//          int value;
//          if (char.IsDigit(c)) {
//             value = c - '0';
//             remainder = (remainder * 10 + value) % 97;
//          }
//          else {
//             // letter: A=10 … Z=35
//             value = c - 'A' + 10;
//             // may be two digits, e.g. 'U' → "30"
//             remainder = (remainder * 100 + value) % 97;
//          }
//       }
//
//       // valid IBANs yield remainder 1
//       return remainder == 1;
//    }
// }

// public static class IbanTestGenerator {
//    private static readonly Random _rnd = new();
//    private static readonly string[] Countries = { "DE", "AT", "CH" };
//    private static readonly (string Country, int BbanLen)[] Specs = {
//       ("DE", 18),  // 8 + 10
//       ("AT", 16),  // 5 + 11
//       ("CH", 17)   // 5 + 12
//    };
//
//    public static string GenerateRandomIban() {
//       
//       // 1) pick a country
//       var spec = Specs[_rnd.Next(Specs.Length)];
//       string country = spec.Country;
//       int bbanLen = spec.BbanLen;
//
//       // 2) random BBAN of digits
//       string bban = new string(Enumerable
//          .Range(0, bbanLen)
//          .Select(_ => (char)('0' + _rnd.Next(10)))
//          .ToArray());
//
//       // 3) placeholder IBAN: CC00 + BBAN
//       string partial = country + "00" + bban;
//
//       // 4) compute the checksum
//       string rearranged = partial.Substring(4) + partial.Substring(0,4);
//       var sb = new StringBuilder();
//       foreach (char c in rearranged) {
//          if (char.IsLetter(c))
//             sb.Append((c - 'A' + 10).ToString());
//          else
//             sb.Append(c);
//       }
//
//       // 5) mod-97
//       int remainder = 0;
//       foreach (char d in sb.ToString()) {
//          remainder = (remainder * 10 + (d - '0')) % 97;
//       }
//
//       int checkDigits = 98 - remainder;
//       string check = checkDigits < 10
//          ? "0" + checkDigits
//          : checkDigits.ToString();
//
//       // 6) assemble final IBAN
//       return country + check + bban;
//    }

   public static string ToIso8601UtcString(this DateTime dateTime) =>
      dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

   public static string ToIso8601TzString(this DateTime dateTime, TimeZoneInfo tz) {
      string formatString = "yyyy-MM-dd'T'HH:mm:sszzz"; 
      var dateTimeOffset = new DateTimeOffset(dateTime, tz.GetUtcOffset(dateTime));
      return dateTimeOffset.ToString("o");
   }
   
   public static (bool, DateTime?, string?) EvalDateTime(string? date) {
      // 1234567890
      // YYYY-MM-dd
      if(date == null || date.Length < 10) 
         return (true, null, $"EvalDateTime: Date format not accepted: {date}");
      
      DateTime dateTime;
      if (!DateTime.TryParseExact(
         s: date,
         format: "yyyy-MM-dd",
         provider: CultureInfo.InvariantCulture,
         style: DateTimeStyles.AdjustToUniversal,
         result: out dateTime)
      ) {
         return (true, null, $"EvalDateTime: Date format not accepted: {date}");
      }
      return (false, dateTime.ToUniversalTime(), null);
   }

}