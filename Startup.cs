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
            //services.AddLogging(config =>
            //{
            //    config.AddConfiguration(Configuration.GetSection("Logging"))
            //        .AddTraceSource(new SourceSwitch("TraceSourceLog", SourceLevels.Verbose.ToString()), logListener)
            //        .AddConsole();
            //});
            //services.AddLogging();
            //services.AddMemoryCache();

            //services.AddCors(); // new

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.Conventions.Add(new VersionByNamespaceConvention());

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader(); // not sure what it is
                options.DefaultApiVersion = new ApiVersion(2,0); // should probably be the latest
            });

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

            services.AddControllers();

            services.AddVersionedApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });

            //services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
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
            // services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(builder => builder.MapControllers());
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{

        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();

        //        // NSWAG
        //        // we may not want to display the whole API to everyone hehe
        //        //app.UseOpenApi();
        //        //app.UseSwaggerUi3();
        //    }

        //    app.UseRouting();
        //    //app.UseResponseCaching();

        //    // global cors policy
        //    //app.UseCors(x => x // new!
        //    //    .AllowAnyOrigin()
        //    //    .AllowAnyMethod()
        //    //    .AllowAnyHeader());

        //    app.UseAuthentication();
        //    app.UseAuthorization();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapControllers();
        //    });
        //}
    }
}
