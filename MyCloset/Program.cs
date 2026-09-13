using System.Reflection;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Implementation;
using MyCloset.Services.Interfaces;
using MyCloset.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Use configuration from environment variables and appsettings.json
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<MyClosetAppDbContext>(options =>
{
    var cosmosEndpoint = configuration["CosmosDb:Endpoint"] ?? configuration["CosmosDb__Endpoint"];
    var cosmosConnectionString = configuration["CosmosDb:ConnectionString"] ?? configuration["CosmosDb__ConnectionString"];
    var databaseName = configuration["CosmosDb:DatabaseName"] ?? configuration["CosmosDb__DatabaseName"] ?? "MyClosetDB";

    // Use managed identity with endpoint if available (production), otherwise use connection string (local dev)
    if (!string.IsNullOrEmpty(cosmosEndpoint))
    {
        options.UseCosmos(
            accountEndpoint: cosmosEndpoint,
            tokenCredential: new DefaultAzureCredential(),
            databaseName: databaseName
        );
    }
    else if (!string.IsNullOrEmpty(cosmosConnectionString))
    {
        options.UseCosmos(
            connectionString: cosmosConnectionString,
            databaseName: databaseName
        );
    }
    else
    {
        throw new InvalidOperationException("CosmosDB configuration is missing. Provide either CosmosDb:Endpoint or CosmosDb:ConnectionString.");
    }
});

builder.Services.AddSingleton(sp =>
{
    var cosmosEndpoint = configuration["CosmosDb:Endpoint"] ?? configuration["CosmosDb__Endpoint"];
    var cosmosConnectionString = configuration["CosmosDb:ConnectionString"] ?? configuration["CosmosDb__ConnectionString"];

    if (!string.IsNullOrEmpty(cosmosEndpoint))
    {
        return new CosmosClient(cosmosEndpoint, new DefaultAzureCredential());
    }

    if (!string.IsNullOrEmpty(cosmosConnectionString))
    {
        return new CosmosClient(cosmosConnectionString);
    }

    throw new InvalidOperationException("CosmosDB configuration is missing. Provide either CosmosDb:Endpoint or CosmosDb:ConnectionString.");
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMyClosetService, MyClosetService>();
builder.Services.AddTransient<IFriendService, FriendService>();
builder.Services.AddTransient<IAIService, AIService>();
builder.Services.AddTransient<ISocialMediaService, SocialMediaService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

// Add HttpClient for services
builder.Services.AddHttpClient();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<CosmosHealthCheck>("cosmosdb", tags: new[] { "db", "ready" });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

// Authentication providers (optional for POC - can be configured later via environment variables)
var authBuilder = builder.Services.AddAuthentication();

var googleClientId = configuration["Authentication:Google:ClientId"] ?? configuration["Authentication__Google__ClientId"];
var googleClientSecret = configuration["Authentication:Google:ClientSecret"] ?? configuration["Authentication__Google__ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

var fbClientId = configuration["Authentication:FB:ClientId"] ?? configuration["Authentication__FB__ClientId"];
var fbClientSecret = configuration["Authentication:FB:ClientSecret"] ?? configuration["Authentication__FB__ClientSecret"];
if (!string.IsNullOrEmpty(fbClientId) && !string.IsNullOrEmpty(fbClientSecret))
{
    authBuilder.AddFacebook(options =>
    {
        options.ClientId = fbClientId;
        options.ClientSecret = fbClientSecret;
    });
}

var msClientId = configuration["Authentication:Microsoft:ClientId"] ?? configuration["Authentication__Microsoft__ClientId"];
var msClientSecret = configuration["Authentication:Microsoft:ClientSecret"] ?? configuration["Authentication__Microsoft__ClientSecret"];
if (!string.IsNullOrEmpty(msClientId) && !string.IsNullOrEmpty(msClientSecret))
{
    authBuilder.AddMicrosoftAccount(options =>
    {
        options.ClientId = msClientId;
        options.ClientSecret = msClientSecret;
    });
}

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My Closet App API",
        Version = "v1",
        Description = "My ASP.NET Core API"
    });

    // Include XML comments (optional, for documenting your API with /// comments)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // Add annotations to your API models and controllers
    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");


    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Closet App V1");
    });

app.Run();