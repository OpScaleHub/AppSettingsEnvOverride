using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AppSettingsEnvOverride.Models;

[ApiController]
[Route("/")]
public class DemoController : ControllerBase
{
    private readonly AppSettings _appSettings;

    public DemoController(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public IActionResult GetConfigurationValues()
    {
        // Log the settings for debugging
        if (_appSettings.DynamicSettings.Count == 0)
        {
            Console.WriteLine("DynamicSettings is empty.");
        }
        else
        {
            foreach (var setting in _appSettings.DynamicSettings)
            {
                Console.WriteLine($"Setting: {setting.Key} = {setting.Value}");
            }
        }

        // Return the settings as a pretty-printed JSON response
        var prettyJson = JsonSerializer.Serialize(_appSettings.DynamicSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Content(prettyJson, "application/json");
    }
}
