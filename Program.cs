using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; // Add this directive
using Microsoft.Extensions.FileProviders; // Add this directive

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables(); // Add environment variables as a configuration source
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false); // Disable file watching
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://*:8080"); // Listen on port 8080
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<HostOptions>(options =>
                {
                    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore; // Configure exception behavior
                });
            })
            .UseDefaultServiceProvider((context, options) =>
            {
                // Replace the default PhysicalFileProvider to disable file watching
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                options.ValidateOnBuild = true;
            });
}
