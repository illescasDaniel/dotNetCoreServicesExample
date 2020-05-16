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
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Person API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Daniel Illescas Romero",
                        Email = "illescasDaniel@protonmail.com",
                        Url = "https://twitter.com/daniel_ir96"
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };
            });

            // configure DI for application services
            // services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // we may not want to display the whole API to everyone hehe
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseRouting();
            //app.UseResponseCaching();

            // global cors policy
            //app.UseCors(x => x // new!
            //    .AllowAnyOrigin()
            //    .AllowAnyMethod()
            //    .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
