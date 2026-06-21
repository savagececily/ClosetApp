using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class AIRecommendationsPage : ContentPage
{
    public AIRecommendationsPage(AIRecommendationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
