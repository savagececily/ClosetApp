using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Models;
using MyCloset.Mobile.Services;

namespace MyCloset.Mobile.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private User? currentUser;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    public ProfileViewModel(IAuthService authService)
    {
        _authService = authService;
        LoadUserProfile();
    }

    private async void LoadUserProfile()
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        if (CurrentUser != null)
        {
            FirstName = CurrentUser.FirstName;
            LastName = CurrentUser.LastName;
            Email = CurrentUser.Email;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Logout", "Are you sure?", "Yes", "No");
        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        // TODO: Implement save profile API call
        await Shell.Current.DisplayAlert("Success", "Profile updated!", "OK");
    }
}
