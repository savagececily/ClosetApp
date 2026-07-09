using System.Reflection;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.OpenApi.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Implementation;
using MyCloset.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration.AddAzureAppConfiguration(options =>
{
    // Check if additional label is available
    string label = Environment.GetEnvironmentVariable("MyClosetAppEnvironment") ?? "Development";

    // Use environment variable for App Configuration endpoint (set by Azure App Service)
    var appConfigEndpoint = Environment.GetEnvironmentVariable("AZURE_APP_CONFIG_ENDPOINT") 
        ?? "https://myclosetapp-appconfig.azconfig.io"; // Fallback for local development

    options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
    .Select(KeyFilter.Any, LabelFilter.Null)
    .Select(KeyFilter.Any, label);
})
.Build();

// Add services to the container.
builder.Services.AddDbContext<MyClosetAppDbContext>(options =>
{
    var cosmosEndpoint = configuration["CosmosDb:Endpoint"];
    var cosmosConnectionString = configuration["CosmosDb:ConnectionString"];
    var databaseName = configuration["CosmosDb:DatabaseName"] ?? "MyClosetDB";

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
    .AddDbContextCheck<MyClosetAppDbContext>("cosmosdb", tags: new[] { "db", "ready" });

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

builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       IConfigurationSection googleAuthNSection =
       configuration.GetSection("Authentication:Google");
       options.ClientId = googleAuthNSection["ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured");
       options.ClientSecret = googleAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is not configured");
   })
   .AddFacebook(options =>
   {
       IConfigurationSection FBAuthNSection =
       configuration.GetSection("Authentication:FB");
       options.ClientId = FBAuthNSection["ClientId"] ?? throw new InvalidOperationException("Facebook ClientId is not configured");
       options.ClientSecret = FBAuthNSection["ClientSecret"] ?? throw new InvalidOperationException("Facebook ClientSecret is not configured");
   })
   .AddMicrosoftAccount(microsoftOptions =>
   {
       microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"] ?? throw new InvalidOperationException("Microsoft ClientId is not configured");
       microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"] ?? throw new InvalidOperationException("Microsoft ClientSecret is not configured");
   });

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