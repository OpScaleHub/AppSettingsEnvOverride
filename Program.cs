using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
void ProcessConfigurationSection(IConfigurationSection section, string path, Dictionary<string, string> settings, ILogger logger)
{
    foreach (var child in section.GetChildren())
    {
        var currentPath = string.IsNullOrEmpty(path) ? child.Key : $"{path}.{child.Key}";

        if (child.GetChildren().Any())
        {
            ProcessConfigurationSection(child, currentPath, settings, logger);
        }
        else
        {
            settings[currentPath] = child.Value;
            logger.LogInformation("Added configuration setting: {Path} = {Value}", currentPath, child.Value);
        }
    }
}

// Configure AppSettings with logging
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
var appSettings = new AppSettings();

try
{
    // Process all settings recursively
    ProcessConfigurationSection(builder.Configuration.GetSection("AppSettings"), "", appSettings.DynamicSettings, logger);

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
                logger.LogInformation("Added environment setting: {Path} = {Value}", settingKey, value);
            }
        }
    }

    // Validate settings
    var validationContext = new ValidationContext(appSettings);
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(appSettings, validationContext, validationResults, true))
    {
        throw new ApplicationException($"Configuration validation failed: {string.Join(", ", validationResults.Select(r => r.ErrorMessage))}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to initialize application settings");
    throw;
}

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton(appSettings);

var app = builder.Build();

// Configure security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
