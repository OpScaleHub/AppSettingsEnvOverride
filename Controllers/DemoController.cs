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
        var prettyJson = JsonSerializer.Serialize(_appSettings.DynamicSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return Content(prettyJson, "application/json");
    }
}
