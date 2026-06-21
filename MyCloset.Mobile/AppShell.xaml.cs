using MyCloset.Mobile.Pages;

namespace MyCloset.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(ClosetPage), typeof(ClosetPage));
        Routing.RegisterRoute(nameof(OutfitsPage), typeof(OutfitsPage));
        Routing.RegisterRoute(nameof(AIRecommendationsPage), typeof(AIRecommendationsPage));
        Routing.RegisterRoute(nameof(SocialMediaPage), typeof(SocialMediaPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
    }
}
