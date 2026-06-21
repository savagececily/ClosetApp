using MyCloset.Mobile.ViewModels;

namespace MyCloset.Mobile.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
