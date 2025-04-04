using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using AppSettingsEnvOverride.Models; // Update namespace reference

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources.
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false); // Load defaults from appsettings.json
builder.Configuration.AddEnvironmentVariables(); // Allow environment variables to override

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings")); // Bind AppSettings section

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.Run();
