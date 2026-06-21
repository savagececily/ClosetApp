using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Services;
using MyCloset.Mobile.Pages;

namespace MyCloset.Mobile.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string selectedProvider = "Google";

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter email and password", "OK");
            return;
        }

        IsLoading = true;
        try
        {
            var (success, message) = await _authService.LoginAsync(Email, Password, SelectedProvider);
            
            if (success)
            {
                // Navigate to main app
                await Shell.Current.GoToAsync("//ClosetPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("Login Failed", message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoginWithProviderAsync(string provider)
    {
        SelectedProvider = provider;
        await Shell.Current.DisplayAlert("OAuth", $"OAuth flow for {provider} would start here", "OK");
        // In production: trigger OAuth flow
    }
}
