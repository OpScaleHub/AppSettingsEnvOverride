using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using AppSettingsEnvOverride.Models;

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
        services.AddEndpointsApiExplorer();

        // Replace the default PhysicalFileProvider to disable file watching globally.
        services.AddSingleton<IFileProvider>(new NullFileProvider());

        // Bind AppSettings section and environment variables with prefix "AppSettings_"
        services.Configure<AppSettings>(options =>
        {
            // Load values from appsettings.json
            var appSettingsSection = _configuration.GetSection("AppSettings");
            foreach (var setting in appSettingsSection.GetChildren())
            {
                options.DynamicSettings[setting.Key] = setting.Value;
                Console.WriteLine($"Loaded from appsettings.json: {setting.Key} = {setting.Value}");
            }

            // Override with environment variables
            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                var key = envVar.ToString();
                if (key.StartsWith("AppSettings_"))
                {
                    var settingKey = key.Substring("AppSettings_".Length);
                    var value = Environment.GetEnvironmentVariable(key);
                    options.DynamicSettings[settingKey] = value;
                    Console.WriteLine($"Overridden by environment variable: {settingKey} = {value}");
                }
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
