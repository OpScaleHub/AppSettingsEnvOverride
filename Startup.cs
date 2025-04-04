using System; // Add this directive
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer(); // Enable API endpoints

        // Replace the default PhysicalFileProvider to disable file watching globally.
        services.AddSingleton<IFileProvider>(new NullFileProvider());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Log a configuration value from environment variables
        var exampleSetting = _configuration["EXAMPLE_SETTING"];
        Console.WriteLine($"EXAMPLE_SETTING: {exampleSetting}");
    }
}
