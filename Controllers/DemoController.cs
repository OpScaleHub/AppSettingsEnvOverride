using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppSettingsEnvOverride.Models;

namespace AppSettingsEnvOverride.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<DemoController> _logger;

        public DemoController(AppSettings appSettings, ILogger<DemoController> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult GetRootConfigurationValues()
        {
            return Ok(new { Settings = _appSettings.DynamicSettings });
        }
    }
}
