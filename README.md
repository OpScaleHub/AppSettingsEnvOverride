# AppSettings Environment Override

A .NET application demonstrating dynamic configuration management with environment variable overrides.

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

## API Documentation

### Swagger UI
The API documentation is available through Swagger UI:
- URL: `http://localhost:<port>/swagger`
- Interactive API documentation and testing interface
- API specification available at: `http://localhost:<port>/swagger/v1/swagger.json`
- To enable Swagger, ensure the application is running with `ASPNETCORE_ENVIRONMENT=Development`:
  ```bash
  export ASPNETCORE_ENVIRONMENT=Development
  dotnet run
  ```

### Health Check
The application includes a health check endpoint:
- URL: `http://localhost:<port>/health`
- Returns HTTP 200 OK when the application is healthy
- Useful for container orchestration and monitoring

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

### Environment Variable Override Format
Override any configuration value using environment variables:
```bash
# Format: AppSettings_[Path]__[SubPath]
AppSettings_Database__ConnectionString="new-value"
AppSettings_Api__Timeout="30"
```

### Nested Configuration Example
For deeply nested configurations:
```json
{
  "AppSettings": {
    "Database": {
      "Server": {
        "Host": "localhost"
      }
    }
  }
}
```
Can be overridden with:
```bash
AppSettings_Database__Server__Host="new-host"
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

## Validation
- All configuration values are validated during startup
- Invalid configurations will prevent application startup
- Check logs for validation errors

## Logging
- All configuration values are logged during startup
- Environment variable overrides are logged separately
- Use log level 'Information' to view configuration loading details