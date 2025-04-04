using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppSettingsEnvOverride.Models
{
    public class AppSettings : IValidatableObject
    {
        public Dictionary<string, string> DynamicSettings { get; set; } = new Dictionary<string, string>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Add any validation rules here
            if (!DynamicSettings.ContainsKey("Foo"))
            {
                results.Add(new ValidationResult("Required setting 'Foo' is missing"));
            }

            return results;
        }
    }
}
