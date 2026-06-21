using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Models;
using MyCloset.Mobile.Services;

namespace MyCloset.Mobile.ViewModels;

public partial class OutfitsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Outfit> outfits = new();

    [ObservableProperty]
    private bool isLoading;

    public OutfitsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadOutfitsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.GetOutfitsAsync();
            if (response.Data != null)
            {
                Outfits = new ObservableCollection<Outfit>(response.Data);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load outfits: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateOutfitAsync()
    {
        await Shell.Current.DisplayAlert("Create Outfit", "Navigate to create outfit page", "OK");
    }

    [RelayCommand]
    private async Task DeleteOutfitAsync(Outfit outfit)
    {
        bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", 
            $"Delete {outfit.Name}?", "Yes", "No");
        
        if (confirm)
        {
            var response = await _apiService.DeleteOutfitAsync(outfit.OutfitId);
            if (response.StatusCode == 200)
            {
                Outfits.Remove(outfit);
            }
        }
    }

    [RelayCommand]
    private async Task RecordWornAsync(Outfit outfit)
    {
        var request = new RecordOutfitWornRequest
        {
            OutfitId = outfit.OutfitId,
            DateWorn = DateTime.Now,
            Occasion = "Casual"
        };
        
        var response = await _apiService.RecordOutfitWornAsync(request);
        await Shell.Current.DisplayAlert("Success", "Outfit recorded!", "OK");
    }
}
