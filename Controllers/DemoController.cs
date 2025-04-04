using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AppSettingsEnvOverride.Models;

namespace AppSettingsEnvOverride.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public DemoController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
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
                return Ok(new { Message = "DynamicSettings is empty" });
            }

            // Return the full DynamicSettings dictionary
            return Ok(_appSettings.DynamicSettings);
        }
    }
}
