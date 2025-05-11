using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpLogging;
namespace BankingApi;

public class Program {
   
   private static void Main(string[] args){
      // path for BankingApi images
      var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
      var wwwroot = Path.Combine(home, "Banking");

      //
      // Configure API Builder and DI-Container
      //
      var builder = WebApplication.CreateBuilder(args);
      builder.Environment.WebRootPath = wwwroot; // path to images;
      
      // Add Logging Services to DI-Container
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
      
      
      
      // add http logging 
      builder.Services.AddHttpLogging(opts =>
         opts.LoggingFields = HttpLoggingFields.All);
      
      // add serialize/deserialize Guid/Guid?  to JSON
      builder.Services.ConfigureHttpJsonOptions(options => {
         options.SerializerOptions.Converters.Add(new NullableGuidConverter());
      });
      
      // add controllers
      builder.Services.AddControllers()
         .AddJsonOptions(opt => {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            opt.JsonSerializerOptions.PropertyNamingPolicy = new LowerCaseNamingPolicy();
            // optional: ignoring null values
            // opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
         });
      
      // add core
      builder.Services.AddCore();
      
      // add Persistence
      builder.Services.AddData(builder.Configuration);
      
      // add ProblemDetails, see https://tools.ietf.org/html/rfc7807
      builder.Services.AddProblemDetails(options => {
         options.CustomizeProblemDetails = context => {
            // Add exception details only in development
            context.ProblemDetails.Extensions["devMode"] = builder.Environment.IsDevelopment();
            if (builder.Environment.IsDevelopment()) {
               // Include stack trace or other debug info in development
               if (context.Exception != null)
                  context.ProblemDetails.Extensions["stackTrace"] = context.Exception.StackTrace;
            }
         };
      });
      
      // add versioning
      builder.Services.AddApiVersioning(2);
      
      // add OpenApi
      //builder.Services.AddOpenApiSettings("Banking API","v1","Übersicht über Bankkonten und Überweisungen.");
      builder.Services.AddOpenApiSettings("Banking API","v2","Übersicht über Bankkonten und Überweisungen.");
      
      // limit imageFile max size
      builder.Services.Configure<FormOptions>(options => {
         // Set the limit to 256 MB
         options.MultipartBodyLengthLimit = 268435456;
      });
      
      // add Cors 
      builder.Services.AddCors(options => {
         options.AddDefaultPolicy(policy => { policy
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
         });
         // options.AddDefaultPolicy(policy => {
         //    policy.WithOrigins(
         //   //    "http://localhost", 
         //         "http://localhost:5010"
         //   //    "https://localhost:7010", 
         //   //    "http://localhost:63342"
         //    ).AllowAnyMethod().AllowAnyHeader();
         // });
      });

      var app = builder.Build();
      
      //
      // Configure Http Pipeline
      //
      // After your middleware registrations but before app.MapControllers()
      // if (app.Environment.IsDevelopment()) {
      //    app.UseDeveloperExceptionPage();
      // } else {
      //    app.UseExceptionHandler();
      // }
      // // use the ProblemDetails middleware
      // app.UseStatusCodePages();

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment()) {
         app.MapOpenApi();
         
         app.UseSwaggerUI(opt => {
            //opt.SwaggerEndpoint("/openapi/v1.json", "Banking API v1");
            opt.SwaggerEndpoint("/openapi/v2.json", "Banking API v2");
         });
      }
      
      app.UseCors();
      
      // app.UseRouting();
      // app.UseAuthentication();
      // app.UseAuthorization();
      app.MapControllers();
      
      app.Run();
   }
}

public class LowerCaseNamingPolicy : JsonNamingPolicy {
   public override string ConvertName(string name) =>
      name.ToLower();
}

public class GuidConverter : JsonConverter<Guid>
{
   public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
   {
      if (reader.TokenType == JsonTokenType.Null)
         return Guid.Empty;
            
      if (reader.TokenType == JsonTokenType.String)
      {
         string? value = reader.GetString();
         return value != null && Guid.TryParse(value, out Guid guid) ? guid : Guid.Empty;
      }
        
      return Guid.Empty;
   }

   public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
   {
      writer.WriteStringValue(value.ToString());
   }
}

public class NullableGuidConverter : JsonConverter<Guid?> {
   public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
      if (reader.TokenType == JsonTokenType.Null)
         return null;

      if (reader.TokenType == JsonTokenType.String &&
          Guid.TryParse(reader.GetString(), out var guid)) {
         return guid;
      }

      throw new JsonException("Invalid Guid format");
   }

   public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options) {
      if (value.HasValue)
         writer.WriteStringValue(value.Value);
      else
         writer.WriteNullValue();
   }
}