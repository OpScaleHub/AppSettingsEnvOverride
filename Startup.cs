using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using AppSettingsEnvOverride.Models; // Update namespace reference

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

        services.Configure<AppSettings>(_configuration.GetSection("AppSettings")); // Bind AppSettings section
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

        // Log configuration values to verify overrides.
        var exampleSetting = _configuration["AppSettings:ExampleSetting"];
        var demoVariable = _configuration["AppSettings:DemoVariable"];
        Console.WriteLine($"ExampleSetting: {exampleSetting}");
        Console.WriteLine($"DemoVariable: {demoVariable}");
    }
}

// AppSettings class to bind configuration values.
public class AppSettings
{
    public string ExampleSetting { get; set; }
    public string DemoVariable { get; set; }
}
