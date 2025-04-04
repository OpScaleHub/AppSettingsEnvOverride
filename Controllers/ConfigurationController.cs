using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppSettingsEnvOverride.Models;
using System;
using System.Linq;

namespace AppSettingsEnvOverride.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<ConfigurationController> _logger;

        public ConfigurationController(AppSettings appSettings, ILogger<ConfigurationController> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        [HttpGet]
        [Route("/")]
        [Route("")] // Also allow /api/configuration
        public IActionResult GetConfigurationValues()
        {
            return Ok(new
            {
                Settings = _appSettings.DynamicSettings,
                DebugInfo = new
                {
                    EnvironmentVariables = Environment.GetEnvironmentVariables()
                        .Cast<System.Collections.DictionaryEntry>()
                        .ToDictionary(
                            entry => entry.Key.ToString(),
                            entry => entry.Value?.ToString()
                        ),
                    Runtime = new
                    {
                        Framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                        OS = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                        ProcessArchitecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString()
                    }
                },
                Timestamp = DateTime.UtcNow
            });
        }
    }
}