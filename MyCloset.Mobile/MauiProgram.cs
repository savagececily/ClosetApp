using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MyCloset.Mobile.Services;
using MyCloset.Mobile.Pages;
using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ClosetViewModel>();
        builder.Services.AddTransient<OutfitsViewModel>();
        builder.Services.AddTransient<AIRecommendationsViewModel>();
        builder.Services.AddTransient<SocialMediaViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ClosetPage>();
        builder.Services.AddTransient<OutfitsPage>();
        builder.Services.AddTransient<AIRecommendationsPage>();
        builder.Services.AddTransient<SocialMediaPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
