using System.Collections.Generic;

namespace AppSettingsEnvOverride.Models
{
    public class AppSettings
    {
        public Dictionary<string, string> DynamicSettings { get; set; } = new Dictionary<string, string>();
    }
}
