using System.Collections.Generic;

namespace AppSettingsEnvOverride.Models
{
    public class AppSettings
    {
        public string Foo { get; set; } // Match the key "Foo" in appsettings.json
        public string Ice { get; set; } // Match the key "Ice" in appsettings.json
        public Dictionary<string, string> DynamicSettings { get; set; } = new();
    }
}
