using System;
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

// Configure AppSettings
var appSettings = new AppSettings();

// Bind static settings from appsettings.json
var section = builder.Configuration.GetSection("AppSettings");
foreach (var child in section.GetChildren())
{
    appSettings.DynamicSettings[child.Key] = child.Value;
    Console.WriteLine($"[Config] Added setting: {child.Key} = {child.Value}");
}

// Add environment variables
foreach (var env in Environment.GetEnvironmentVariables().Keys)
{
    var key = env.ToString();
    if (key.StartsWith("AppSettings_"))
    {
        var settingKey = key.Substring("AppSettings_".Length);
        var value = Environment.GetEnvironmentVariable(key);
        appSettings.DynamicSettings[settingKey] = value;
        Console.WriteLine($"[Env] Added setting: {settingKey} = {value}");
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
