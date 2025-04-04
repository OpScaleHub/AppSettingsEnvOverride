using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AppSettingsEnvOverride.Models;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();

// Helper method to process configuration sections recursively
void ProcessConfigurationSection(IConfigurationSection section, string path, Dictionary<string, string> settings)
{
    foreach (var child in section.GetChildren())
    {
        var currentPath = string.IsNullOrEmpty(path) ? child.Key : $"{path}.{child.Key}";

        if (child.GetChildren().Any())
        {
            ProcessConfigurationSection(child, currentPath, settings);
        }
        else
        {
            settings[currentPath] = child.Value;
            Console.WriteLine($"[Config] Added setting: {currentPath} = {child.Value}");
        }
    }
}

// Configure AppSettings
var appSettings = new AppSettings();

// Process all settings recursively
ProcessConfigurationSection(builder.Configuration.GetSection("AppSettings"), "", appSettings.DynamicSettings);

// Add environment variables (now supporting dot notation for nested settings)
foreach (var env in Environment.GetEnvironmentVariables().Keys)
{
    var key = env.ToString();
    if (key.StartsWith("AppSettings_"))
    {
        var settingKey = key.Substring("AppSettings_".Length).Replace("__", ".");
        var value = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrEmpty(value))
        {
            appSettings.DynamicSettings[settingKey] = value;
            Console.WriteLine($"[Env] Added setting: {settingKey} = {value}");
        }
    }
}

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton(appSettings);

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.Run();
