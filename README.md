# AppSettings Environment Override

This application demonstrates how to use hierarchical configuration in .NET with the ability to override settings using environment variables.

## Configuration Structure

The application uses a base configuration from `appsettings.json`:

```json
{
  "AppSettings": {
    "Foo": "Bar",
    "Baz": "Qux",
    "nested": {
      "Foo": "Bar",
      "Baz": "Qux"
    }
  }
}
```

## Environment Variable Override

You can override any setting using environment variables by following these naming conventions:
- Use `_` as section delimiter
- Use `__` (double underscore) for nested objects

### Examples:

1. Override top-level setting:
   ```bash
   AppSettings_Foo=BarBar
   ```

2. Override nested setting:
   ```bash
   AppSettings_nested__Baz=BazBaz
   ```

## Running with Docker

```bash
docker run --rm --publish 8080:8080 \
  --env AppSettings_Foo=BarBar \
  --env AppSettings_nested__Baz=BazBaz \
  -it ghcr.io/opscalehub/appsettingsenvoverride:main
```

## Example Output

Making a request to the application shows both the configuration values and debug information:

```bash
$ http http://localhost:8080/api/configuration
```

Response:
```json
{
    "settings": {
        "Baz": "Qux",
        "Foo": "BarBar",           # Overridden by environment variable
        "nested.Baz": "BazBaz",    # Overridden by environment variable
        "nested.Foo": "Bar"
    },
    "debugInfo": {
        // ... debug info ...
    },
    "timestamp": "2025-04-04T18:55:24.5121304Z"
}
```

## How It Works

1. Base configuration is loaded from `appsettings.json`
2. Environment variables are automatically mapped to configuration values
3. Environment variables take precedence over file-based settings
4. Nested settings use `__` as a delimiter in environment variables

This allows for flexible configuration management where base settings can be provided in files while specific overrides can be injected through environment variables, making it ideal for containerized deployments.