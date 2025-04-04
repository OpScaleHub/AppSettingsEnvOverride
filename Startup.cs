using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using AppSettingsEnvOverride.Models;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<Startup> _logger;

    public Startup(IConfiguration configuration, ILogger<Startup> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSingleton<IFileProvider>(new NullFileProvider());

        // Configure AppSettings with both appsettings.json and environment variables
        var appSettings = new AppSettings();

        // First bind static settings
        var section = _configuration.GetSection("AppSettings");
        foreach (var child in section.GetChildren())
        {
            appSettings.DynamicSettings[child.Key] = child.Value;
            _logger.LogInformation($"Added config setting: {child.Key} = {child.Value}");
        }

        // Then overlay environment variables (they take precedence)
        foreach (var env in Environment.GetEnvironmentVariables().Keys)
        {
            var key = env.ToString();
            if (key.StartsWith("AppSettings_"))
            {
                var settingKey = key.Substring("AppSettings_".Length);
                var value = Environment.GetEnvironmentVariable(key);
                appSettings.DynamicSettings[settingKey] = value;
                _logger.LogInformation($"Added env setting: {settingKey} = {value}");
            }
        }

        // Register AppSettings with the correct type
        services.AddSingleton<AppSettingsEnvOverride.Models.AppSettings>(appSettings);
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

            // Redirect root path to the DemoController's GetConfigurationValues endpoint
            endpoints.MapGet("/", async context =>
            {
                context.Response.Redirect("/Demo/GetConfigurationValues");
            });
        });
    }
}
