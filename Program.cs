using System; // Add this directive
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory, // Set to avoid default file watchers
    ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
});

// Clear default configuration sources to avoid file watchers.
builder.Configuration.Sources.Clear();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false); // Disable file watching
builder.Configuration.AddEnvironmentVariables();

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
