using System;
using System.Collections.Generic;
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

        // Bind AppSettings section and environment variables with prefix "AppSettings_"
        services.Configure<AppSettings>(options =>
        {
            _configuration.GetSection("AppSettings").Bind(options.DynamicSettings);

            foreach (var envVar in Environment.GetEnvironmentVariables().Keys)
            {
                var key = envVar.ToString();
                if (key.StartsWith("AppSettings_"))
                {
                    var settingKey = key.Substring("AppSettings_".Length);
                    options.DynamicSettings[settingKey] = Environment.GetEnvironmentVariable(key);
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

        // Log configuration values to verify overrides.
        var exampleSetting = _configuration["AppSettings:ExampleSetting"];
        var demoVariable = _configuration["AppSettings:DemoVariable"];
        Console.WriteLine($"ExampleSetting: {exampleSetting}");
        Console.WriteLine($"DemoVariable: {demoVariable}");

        // Log configuration values to verify overrides.
        var fooValue = _configuration["AppSettings:Foo"];
        Console.WriteLine($"Foo: {fooValue}");
    }
}

// AppSettings class to bind configuration values.
public class AppSettings
{
    public string ExampleSetting { get; set; }
    public string DemoVariable { get; set; }
    public Dictionary<string, string> DynamicSettings { get; set; } = new Dictionary<string, string>();
}
