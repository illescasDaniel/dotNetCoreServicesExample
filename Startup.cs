using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Extensions.Logging;
using myMicroservice.Helpers;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
//using NSwag.AspNetCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
//using NSwag.Generation.Processors.Security;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData;
using Microsoft.OData;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.Net.Http.Headers;

namespace myMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(Log.Logger, false));

            //services.AddLogging();
            //services.AddMemoryCache();

            //services.AddCors(); // new

            // Map config section stuff to classes
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<Properties.AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<Properties.AppSettings>();
            var secret = appSettings.JwtSecret;

            if (secret == null)
            {
                throw new Exception("JWT Secret is null!");
            }

            var key = Encoding.ASCII.GetBytes(secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            // OData
            //services.AddMvcCore(options =>
            //{
            //    options.EnableEndpointRouting = false; // OData
            //});
            //services.AddOData();
            // end - Odata

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.Conventions.Add(new VersionByNamespaceConvention());

                // if a service has a route that doesn't specify a version, assume that is v2 by default [the latest]
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options); // latest version
                //options.DefaultApiVersion = new ApiVersion(1,0); // Default version

                //options.ApiVersionReader = new UrlSegmentApiVersionReader();
                // if you want to specify the version via query string: options.ApiVersionReader = new QueryStringApiVersionReader();
            });

            #region OData + api versioning
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddOData().EnableApiVersioning();

            services.AddODataApiExplorer(
                options =>
                {
                    //options.DefaultApiVersion = new ApiVersion(1, 0);
                    //options.ApiVersionParameterSource = new UrlSegmentApiVersionReader();
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;

                    // configure query options (which cannot otherwise be configured by OData conventions)
                    //options.QueryOptions.Controller<Database.Odata.UsersController>().
                    //.Action(c => c.Get(default));
                    //.Allow(AllowedQueryOptions.Skip | AllowedQueryOptions.Count)
                    //.AllowTop(100)
                    //.AllowOrderBy("firstName", "lastName");
                });

            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
            #endregion

            services.AddHealthChecks(); // not sure

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(config =>
            {

                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "My API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Daniel Illescas Romero",
                        Email = "illescas.daniel@protonmail.com",
                        Url = new Uri("https://github.com/illescasDaniel"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MPL 2.0",
                        Url = new Uri("https://www.mozilla.org/en-US/MPL/2.0/"),
                    }
                });
                config.SwaggerDoc("v1-odata", new OpenApiInfo
                {
                    Version = "v1-odata",
                    Title = "My API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Daniel Illescas Romero",
                        Email = "illescas.daniel@protonmail.com",
                        Url = new Uri("https://github.com/illescasDaniel"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MPL 2.0",
                        Url = new Uri("https://www.mozilla.org/en-US/MPL/2.0/"),
                    }
                });
                config.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "My API",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Daniel Illescas Romero",
                        Email = "illescas.daniel@protonmail.com",
                        Url = new Uri("https://github.com/illescasDaniel"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MPL 2.0",
                        Url = new Uri("https://www.mozilla.org/en-US/MPL/2.0/"),
                    }
                });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            config.IncludeXmlComments(xmlPath);

            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.\nExample: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {} // Default tokens? doesn't seem to work
                }});
            });

            // SWAGGER - NSWAG
            //services.AddSwaggerDocument(config =>
            //{
            //    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
            //    config.AddSecurity("JWT token", Enumerable.Empty<string>(),
            //        new NSwag.OpenApiSecurityScheme()
            //        {
            //            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            //            Name = "Authorization",
            //            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            //            Description = "Bearer {my long token}"
            //        }
            //    );

            //    config.PostProcess = document =>
            //    {
            //        document.Info.Version = "v1";
            //        document.Info.Title = "Person API";
            //        document.Info.Description = "A simple ASP.NET Core web API";
            //        document.Info.TermsOfService = "None";
            //        document.Info.Contact = new NSwag.OpenApiContact
            //        {
            //            Name = "Daniel Illescas Romero",
            //            Email = "illescasDaniel@protonmail.com",
            //            Url = "https://twitter.com/daniel_ir96"
            //        };
            //        document.Info.License = new NSwag.OpenApiLicense
            //        {
            //            Name = "Use under LICX",
            //            Url = "https://example.com/license"
            //        };
            //    };
            //});

            // configure DI for application services
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<Database.DatabaseContext>();
        }

        // Odata - no api versioning
        //private static IEdmModel GetEdmModel()
        //{
        //    var builder = new ODataConventionModelBuilder();
        //    builder.EntitySet<Database.Entities.User>("Users");
        //    return builder.GetEdmModel();
        //}

        #region Odata + versioning
        public void Configure(IApplicationBuilder app, VersionedODataModelBuilder modelBuilder, IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.ServiceProvider.GetRequiredService<ODataOptions>().UrlKeyDelimiter = ODataUrlKeyDelimiter.Slash;

                app.UseODataBatching();

                // This should enable "odata" and normal "rest api" for the same controllers
                //routeBuilder.EnableDependencyInjection();

                // this enables all these operations for all types
                //routeBuilder.Select().Expand().Count().Filter().OrderBy().MaxTop(100).SkipToken().Build();

                // maps model configured on "Database.Odata.Configurations"
                routeBuilder.MapVersionedODataRoutes("odata", "odata", modelBuilder.GetEdmModels());
            });

            app.UseEndpoints(builder => builder.MapControllers());

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }
        #endregion

        //public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        //{
        //    //app.UseHttpsRedirection();

        //    app.UseRouting();

        //    app.UseAuthentication();
        //    app.UseAuthorization();

        //    app.UseEndpoints(builder => builder.MapControllers());

        //    app.UseSwagger();
        //    app.UseSwaggerUI(options =>
        //    {
        //        // build a swagger endpoint for each discovered API version
        //        foreach (var description in provider.ApiVersionDescriptions)
        //        {
        //            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        //        }
        //    });

        //    // OData
        //    //app.UseMvc(routerBuilder =>
        //    //{
        //    //    routerBuilder.EnableDependencyInjection(); // enables non-odata calls
        //    //    routerBuilder.Select().Expand().Count().Filter().OrderBy().MaxTop(100).SkipToken().Build();
        //    //    routerBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
        //    //});
        //    // end - OData
        //}
    }
}
