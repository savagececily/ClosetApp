using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class SocialMediaPage : ContentPage
{
    private readonly SocialMediaViewModel _viewModel;

    public SocialMediaPage(SocialMediaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadConnectedAccountsCommand.ExecuteAsync(null);
        await _viewModel.LoadOutfitHistoryCommand.ExecuteAsync(null);
    }
}
