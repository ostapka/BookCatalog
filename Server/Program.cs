using AspNetCoreRateLimit;
using BookCatalog.Server.AppCore;
using BookCatalog.Server.Infrastructure;
using BookCatalog.Server.ModelBinders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add SignalR
builder.Services.AddSignalR();

// Add services to the container
builder.Services.AddControllersWithViews(config =>
{
    config.ModelBinderProviders.Insert(0, new SortModelBinderProvider(config.ModelBinderProviders));
});
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer(); // For minimal APIs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My .NET 8 API",
        Version = "v1",
        Description = "A simple API example built with .NET 8"
    });

    // Optionally include XML comments (enable in csproj first)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Add App Core and AutoMapper
builder.Services.AddAppCore();
builder.Services.AddAutoMapper(typeof(BookProfile));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
});

var app = builder.Build();

// Apply migrations and seed data
SeedWrapper.Seed(app.Services);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // Recommended to keep after UseExceptionHandler in production
    app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My .NET 8 API v1");
    options.RoutePrefix = string.Empty; // Swagger UI at the app's root
});

// Apply CORS before routing and other middleware
app.UseCors("AllowAll");

// Rate Limiting comes after CORS
app.UseIpRateLimiting();

// Static files should be served after the CORS and Rate Limiting checks
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();

// Routing should come after static files
app.UseRouting();

// Map endpoints
app.MapRazorPages();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger")); // Redirect root to Swagger UI (optional)
app.MapHub<BookHub>("/bookhub");

// Fallback for Blazor WebAssembly
app.MapFallbackToFile("index.html");

app.Run();