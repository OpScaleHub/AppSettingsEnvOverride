using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration; // Add this directive

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources.
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
    config.AddEnvironmentVariables();
});

// Replace the default PhysicalFileProvider to disable file watching globally.
builder.Services.AddSingleton<IFileProvider>(new NullFileProvider());

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.Run();

// Custom NullFileProvider to disable file watching.
public class NullFileProvider : IFileProvider
{
    public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
    public IFileInfo GetFileInfo(string subpath) => new NotFoundFileInfo(subpath);
    public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
}
