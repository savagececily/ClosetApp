using System.Reflection;
using Azure.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
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

    options.Connect(new Uri("https://myclosetapp-appconfig.azconfig.io"), new DefaultAzureCredential())
    .Select(KeyFilter.Any, LabelFilter.Null)
    .Select(KeyFilter.Any, label);
})
.Build();

// Add services to the container.
builder.Services.AddDbContext<MyClosetAppDbContext>(options =>
{
    var connectionString = configuration["MyClosetAppDB"];

    options.UseSqlServer(connectionString);
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMyClosetService, MyClosetService>();
builder.Services.AddTransient<IFriendService, FriendService>();
builder.Services.AddSingleton<IBlobStorageService, BlobStorageService>();

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
       options.ClientId = googleAuthNSection["ClientId"];
       options.ClientSecret = googleAuthNSection["ClientSecret"];
   })
   .AddFacebook(options =>
   {
       IConfigurationSection FBAuthNSection =
       configuration.GetSection("Authentication:FB");
       options.ClientId = FBAuthNSection["ClientId"];
       options.ClientSecret = FBAuthNSection["ClientSecret"];
   })
   .AddMicrosoftAccount(microsoftOptions =>
   {
       microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
       microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
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

app.MapFallbackToFile("index.html");;

app.Run();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});