using Asp.Versioning;
using BankingApi.Core;
using BankingApi.Core.UseCases;
using BankingApi.Data;
using BankingApi.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization.Metadata;

namespace BankingApi;

public static class ExtensionsServices {
   #region AddData
   public static IServiceCollection AddData(
      this IServiceCollection services,
      IConfiguration configuration
   ) {
      services.AddScoped<IOwnersRepository, OwnersRepository>();
      services.AddScoped<IAccountsRepository, AccountsRepository>();
      services.AddScoped<IBeneficiariesRepository, BeneficiariesRepository>();
      services.AddScoped<ITransfersRepository, TransfersRepository>();
      services.AddScoped<ITransactionsRepository, TransactionsRepository>();

      // Add DbContext (Database) to DI-Container
      var (useDatabase, dataSource) = DataContext.EvalDatabaseConfiguration(configuration);

      switch (useDatabase) {
         case "Sqlite":
         case "SqliteInMemory":
            services.AddDbContext<IDataContext, DataContext>(options =>
               options.UseSqlite(dataSource)
            );
            break;
         default:
            throw new Exception("appsettings.json UseDatabase not available");
      }
      return services;
   }
   #endregion

   #region AddCore
   public static IServiceCollection AddCore(
      this IServiceCollection services
   ) {
      services.AddScoped<IUseCasesTransfer, UseCasesTransfer>();
      return services;
   }
   #endregion

   #region AddApiVersioning
   public static IServiceCollection AddApiVersioning(
      this IServiceCollection services,
      int startVersion = 1
   ) {
      var apiVersionReader = ApiVersionReader.Combine(
         new UrlSegmentApiVersionReader(),
         new HeaderApiVersionReader("x-api-version")
         // new MediaTypeApiVersionReader("x-api-version"),
         // new QueryStringApiVersionReader("api-version")
      );

      services.AddApiVersioning(options => {
         options.AssumeDefaultVersionWhenUnspecified = true;
         options.DefaultApiVersion = new ApiVersion(startVersion, 0);
         options.ReportApiVersions = true;
         options.ApiVersionReader = apiVersionReader;
      }).AddApiExplorer(options => {
         options.GroupNameFormat = "'v'VVV";
         options.SubstituteApiVersionInUrl = true;
      });
      return services;
   }
   #endregion

   #region AddOpenApiSettings
   public static IServiceCollection AddOpenApiSettings(
      this IServiceCollection services,
      string title,
      string version,
      string description
   ) {
      
      services.AddOpenApi(version, opt => {
         opt.AddDocumentTransformer((document, _, _) => {
            document.Info = new() {
               Title = title,
               Version = version,
               Description = description
            };
            return Task.CompletedTask;
         });
      });
      
      /*
      opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
      opt.AddDocumentTransformer((document, context, cancellationToken) => {
         var requirements = new Dictionary<string, OpenApiSecurityScheme> {
            ["Bearer"] = new OpenApiSecurityScheme {
               Type = SecuritySchemeType.Http,
               Scheme = "bearer", // "bearer" refers to the header name here
               In = ParameterLocation.Header,
               BearerFormat = "Json Web Token"
            }
         };
         document.Components ??= new OpenApiComponents();
         document.Components.SecuritySchemes = requirements;
         return Task.CompletedTask;
      });
      */
      
      return services;
   }
   #endregion

   
   /*
   #region AddOpenApiSettings
   public static IServiceCollection AddOpenApiSettings(
      this IServiceCollection services,
      string title,
      string version,
      string description
   ) {
      services.AddOpenApi(version, options => {
         
         // 1) Ensure Guid and Guid? share the same schema ID ("Guid")
         options.CreateSchemaReferenceId = typeInfo => {
            if (typeInfo.Type == typeof(Guid) || typeInfo.Type == typeof(Guid?)) {
               return "Guid";
            }
            return OpenApiOptions.CreateDefaultSchemaReferenceId(typeInfo);
         };

         // 2) Define the shared "Guid" schema (type + format)
         options.AddDocumentTransformer((document, context, cancellationToken) => {
            
            document.Components ??= new OpenApiComponents();
            document.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

            document.Components.Schemas["Guid"] = new OpenApiSchema {
               Type = "string",
               Format = "uuid"
               // Note: nullable flag is handled in the schema transformer below
            };
            
            document.Info = new OpenApiInfo {
               Title = title,
               Version = version,
               Description = description
            };
            
            return Task.CompletedTask;
         });

         // 3) For each schema, look at context.JsonTypeInfo.Type to detect Guid vs Guid?
         options.AddSchemaTransformer((
            OpenApiSchema schema,
            OpenApiSchemaTransformerContext context,
            CancellationToken ct
         ) => {
            var clrType = context.JsonTypeInfo.Type; // <— use JsonTypeInfo.Type, not context.Type or GetType()
            if (clrType == typeof(Guid) || clrType == typeof(Guid?)) {
               schema.Type = "string";
               schema.Format = "uuid";
               schema.Nullable = (clrType == typeof(Guid?));
            }
            return Task.CompletedTask;
         });
      });

      // options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
      // options.AddDocumentTransformer((document, context, cancellationToken) => {
      //    var requirements = new Dictionary<string, OpenApiSecurityScheme> {
      //       ["Bearer"] = new OpenApiSecurityScheme {
      //          Type = SecuritySchemeType.Http,
      //          Scheme = "bearer", // "bearer" refers to the header name here
      //          In = ParameterLocation.Header,
      //          BearerFormat = "Json Web Token"
      //       }
      //    };
      //    document.Components ??= new OpenApiComponents();
      //    document.Components.SecuritySchemes = requirements;
      //    return Task.CompletedTask;
      // });

      return services;
   }
   #endregion
   */

   #region AddSwaggerGenSettings
   public static IServiceCollection AddSwaggerSettings(
      this IServiceCollection services,
      string title,
      string version,
      string description
   ) {
      services.AddSwaggerGen(c => {
         // 1) Set up your document info
         c.SwaggerDoc(version, new OpenApiInfo {
            Title       = title,
            Version     = version,
            Description = description
         });

         // 2) Tell Swashbuckle how to render Guid  →  string(uuid)
         c.MapType<Guid>(() => new OpenApiSchema {
            Type   = "string",
            Format = "uuid"
         });

         // 3) And Guid? → nullable string(uuid)
         c.MapType<Guid?>(() => new OpenApiSchema {
            Type     = "string",
            Format   = "uuid",
            Nullable = true
         });

         // …any other customizations you need…
      });

      return services;
   }
   #endregion
}

internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
   : IOpenApiDocumentTransformer {
   public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
      CancellationToken cancellationToken) {
      var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
      if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer")) {
         var requirements = new Dictionary<string, OpenApiSecurityScheme> {
            ["Bearer"] = new OpenApiSecurityScheme {
               Type = SecuritySchemeType.Http,
               Scheme = "bearer", // "bearer" refers to the header name here
               In = ParameterLocation.Header,
               BearerFormat = "Json Web Token"
            }
         };
         document.Components ??= new OpenApiComponents();
         document.Components.SecuritySchemes = requirements;
      }
   }
}

// public static IServiceCollection AddOpenApiSettings(
//    this IServiceCollection services,
//    string version
// ) {
//    // Add OpenAPI with Bearer token support
//    services.AddOpenApi(version, settings => {
//   
//       settings.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
//          Type = SecuritySchemeType.Http,
//          Scheme = "bearer",
//          BearerFormat = "JWT",
//          Description = "JWT Authorization header using the Bearer scheme."
//       });
//       settings.AddSecurityRequirement(new OpenApiSecurityRequirement {
//          {
//             new OpenApiSecurityScheme {
//                Reference = new OpenApiReference {
//                   Type = ReferenceType.SecurityScheme,
//                   Id = "Bearer"
//                }
//             },
//             Array.Empty<string>()
//          }
//       });
//    });
//    return services;
// }