using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class OutfitsPage : ContentPage
{
    private readonly OutfitsViewModel _viewModel;

    public OutfitsPage(OutfitsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadOutfitsCommand.ExecuteAsync(null);
    }
}
