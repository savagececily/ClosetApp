using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
