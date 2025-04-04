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

        // Replace the default PhysicalFileProvider to disable file watching globally.
        services.AddSingleton<IFileProvider>(new NullFileProvider());

        // Configure AppSettings and populate DynamicSettings
        services.Configure<AppSettings>(options =>
        {
            // Bind static settings from appsettings.json
            var appSettingsSection = _configuration.GetSection("AppSettings");
            foreach (var setting in appSettingsSection.GetChildren())
            {
                options.DynamicSettings[setting.Key] = setting.Value;
                _logger.LogDebug($"Added static setting: {setting.Key} = {setting.Value}");
            }

            // Log static settings
            _logger.LogInformation("Static settings from appsettings.json:");
            foreach (var setting in options.DynamicSettings)
            {
                _logger.LogInformation($"StaticSetting: {setting.Key} = {setting.Value}");
            }

            // Merge environment variables into DynamicSettings
            _logger.LogInformation("Environment variables with prefix 'AppSettings_':");
            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                var key = envVar.ToString();
                if (key.StartsWith("AppSettings_"))
                {
                    var settingKey = key.Substring("AppSettings_".Length);
                    var value = Environment.GetEnvironmentVariable(key);
                    if (!string.IsNullOrEmpty(settingKey) && value != null)
                    {
                        _logger.LogInformation($"EnvVariable: {settingKey} = {value}");
                        options.DynamicSettings[settingKey] = value;
                        _logger.LogDebug($"Merged environment variable: {settingKey} = {value}");
                    }
                }
            }

            // Log the final DynamicSettings content
            _logger.LogInformation("Final DynamicSettings content:");
            foreach (var setting in options.DynamicSettings)
            {
                _logger.LogInformation($"DynamicSetting: {setting.Key} = {setting.Value}");
            }
        });
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

// AppSettings class to bind configuration values.
public class AppSettings
{
    public Dictionary<string, string> DynamicSettings { get; set; } = new Dictionary<string, string>();
}
