using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AppSettingsEnvOverride.Models;
using System;
using Microsoft.Extensions.Logging;

namespace AppSettingsEnvOverride.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<DemoController> _logger;

        public DemoController(IOptions<AppSettings> appSettings, ILogger<DemoController> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;

            // Log the contents of DynamicSettings during controller initialization
            if (_appSettings.DynamicSettings.Count == 0)
            {
                _logger.LogWarning("DynamicSettings is empty in DemoController constructor.");
            }
            else
            {
                foreach (var setting in _appSettings.DynamicSettings)
                {
                    _logger.LogInformation($"DynamicSetting in DemoController: {setting.Key} = {setting.Value}");
                }
            }
        }

        [HttpGet("GetConfigurationValues")]
        public IActionResult GetConfigurationValues()
        {
            // Return the full DynamicSettings dictionary
            return Ok(_appSettings.DynamicSettings);
        }

        [HttpGet("/")]
        public IActionResult GetRootConfigurationValues()
        {
            if (_appSettings.DynamicSettings.Count == 0)
            {
                return Ok(new
                {
                    Message = "DynamicSettings is empty",
                    DebugInfo = new
                    {
                        EnvironmentVariables = Environment.GetEnvironmentVariables()
                    }
                });
            }

            // Return the full DynamicSettings dictionary
            return Ok(_appSettings.DynamicSettings);
        }
    }
}
