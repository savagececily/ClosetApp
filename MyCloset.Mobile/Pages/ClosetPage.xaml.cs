using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class ClosetPage : ContentPage
{
    private readonly ClosetViewModel _viewModel;

    public ClosetPage(ClosetViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadItemsCommand.ExecuteAsync(null);
    }
}
