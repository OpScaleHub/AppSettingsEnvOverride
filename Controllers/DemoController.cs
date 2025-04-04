using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

[ApiController]
[Route("demo")]
public class DemoController : ControllerBase
{
    [HttpGet]
    public IActionResult GetEnvironmentVariables()
    {
        var envVars = new Dictionary<string, string>
        {
            { "EXAMPLE_SETTING", Environment.GetEnvironmentVariable("EXAMPLE_SETTING") ?? "Not Set" },
            { "DEMO_VARIABLE", Environment.GetEnvironmentVariable("DEMO_VARIABLE") ?? "Not Set" }
        };

        return Ok(envVars);
    }
}
