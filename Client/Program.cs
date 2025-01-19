using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using BookCatalog.Client;
using BookCatalog.Client.Configuration;
using BookCatalog.Client.Interfaces;
using BookCatalog.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorise(options =>
{
    options.Immediate = true;
})
.AddBootstrap5Providers()
.AddFontAwesomeIcons();

builder.Services.AddScoped<IBookService, BookService>();

// Fetch appsettings.json manually
var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var configResponse = await httpClient.GetAsync("appsettings.json");

if (!configResponse.IsSuccessStatusCode)
{
    throw new InvalidOperationException("Could not load appsettings.json");
}

var configJson = await configResponse.Content.ReadAsStringAsync();
var configuration = new ConfigurationBuilder()
    .AddJsonStream(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(configJson)))
    .Build();

// Bind the configuration only for Docker
var apiConfig = configuration.GetSection("ApiConfig").Get<Configuration>();

if (apiConfig is null)
{
    throw new InvalidOperationException("Failed to bind ApiConfig section to Configuration class");
}

// Load logging configuration
var loggingConfig = configuration.GetSection("Logging:LogLevel").Get<Dictionary<string, string>>();

if (loggingConfig is null)
{
    throw new InvalidOperationException("Failed to get LoggingConfig section");
}

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiConfig.ApiBaseUrl!) });

if (loggingConfig is not null)
{
    foreach (var (category, level) in loggingConfig)
    {
        if (Enum.TryParse<LogLevel>(level, out var logLevel))
        {
            builder.Logging.AddFilter(category, logLevel);
        }
    }
}

builder.Services.AddSingleton(sp => new HubConnectionBuilder()
    .WithUrl($"{apiConfig.ApiBaseUrl}/bookhub")
    .WithAutomaticReconnect()
    .Build());

await builder.Build().RunAsync();
