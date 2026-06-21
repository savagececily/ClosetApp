using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Models;
using MyCloset.Mobile.Services;

namespace MyCloset.Mobile.ViewModels;

public partial class SocialMediaViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<SocialMediaConnection> connectedAccounts = new();

    [ObservableProperty]
    private ObservableCollection<OutfitHistory> outfitHistory = new();

    [ObservableProperty]
    private bool isLoading;

    public SocialMediaViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadConnectedAccountsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.GetConnectedAccountsAsync();
            if (response.Data != null)
            {
                ConnectedAccounts = new ObservableCollection<SocialMediaConnection>(response.Data);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadOutfitHistoryAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.GetOutfitHistoryAsync();
            if (response.Data != null)
            {
                OutfitHistory = new ObservableCollection<OutfitHistory>(response.Data);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LinkAccountAsync(string platform)
    {
        await Shell.Current.DisplayAlert("Link Account", 
            $"OAuth flow for {platform} would be triggered here", "OK");
    }

    [RelayCommand]
    private async Task DisconnectAccountAsync(SocialMediaConnection account)
    {
        bool confirm = await Shell.Current.DisplayAlert("Confirm", 
            $"Disconnect {account.Platform}?", "Yes", "No");
        
        if (confirm)
        {
            var response = await _apiService.DisconnectAccountAsync(account.ConnectionId);
            if (response.StatusCode == 200)
            {
                ConnectedAccounts.Remove(account);
            }
        }
    }

    [RelayCommand]
    private async Task ViewAnalyticsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.GetOutfitAnalyticsAsync();
            await Shell.Current.DisplayAlert("Analytics", "View analytics details here", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
