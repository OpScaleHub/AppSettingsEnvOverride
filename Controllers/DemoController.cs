using System; // Add this directive
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AppSettingsEnvOverride.Models; // Ensure correct namespace

[ApiController]
[Route("demo")]
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
        return Ok(new
        {
            ExampleSetting = _appSettings.ExampleSetting,
            DemoVariable = _appSettings.DemoVariable,
            RuntimeEnvironmentVariables = new
            {
                ExampleSettingEnv = Environment.GetEnvironmentVariable("EXAMPLE_SETTING") ?? "Not Set",
                DemoVariableEnv = Environment.GetEnvironmentVariable("DEMO_VARIABLE") ?? "Not Set"
            }
        });
    }
}
