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
            _configuration.GetSection("AppSettings").Bind(options);

            // Merge environment variables into DynamicSettings
            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                var key = envVar.ToString();
                if (key.StartsWith("AppSettings_"))
                {
                    var settingKey = key.Substring("AppSettings_".Length);
                    var value = Environment.GetEnvironmentVariable(key);
                    if (!string.IsNullOrEmpty(settingKey) && value != null)
                    {
                        options.DynamicSettings[settingKey] = value;
                    }
                }
            }

            // Add static settings to DynamicSettings
            foreach (var property in typeof(AppSettings).GetProperties())
            {
                var key = property.Name;
                var value = property.GetValue(options)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    options.DynamicSettings[key] = value;
                }
            }

            // Log the final DynamicSettings content
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
    public string ExampleSetting { get; set; }
    public string DemoVariable { get; set; }
    public Dictionary<string, string> DynamicSettings { get; set; } = new Dictionary<string, string>();
}
