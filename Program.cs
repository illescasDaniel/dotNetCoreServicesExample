using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace myMicroservice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "server.log",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                fileSizeLimitBytes: 300 * 1024 * 124,
                rollOnFileSizeLimit: true
            )
            .WriteTo.File(
                path: "server-errors.log",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
                fileSizeLimitBytes: 300 * 1024 * 124,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
