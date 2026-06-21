using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Models;
using MyCloset.Mobile.Services;

namespace MyCloset.Mobile.ViewModels;

public partial class ClosetViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<ClothingItem> clothingItems = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private string selectedCategory = "All";

    public ClosetViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.GetClothingItemsAsync();
            if (response.Data != null)
            {
                ClothingItems = new ObservableCollection<ClothingItem>(response.Data);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load items: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        // Navigate to add item page (to be implemented)
        await Shell.Current.DisplayAlert("Add Item", "Navigate to add item page", "OK");
    }

    [RelayCommand]
    private async Task DeleteItemAsync(ClothingItem item)
    {
        bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", 
            $"Delete {item.Title}?", "Yes", "No");
        
        if (confirm)
        {
            var response = await _apiService.DeleteClothingItemAsync(item.ClothingItemId);
            if (response.StatusCode == 200)
            {
                ClothingItems.Remove(item);
            }
        }
    }

    [RelayCommand]
    private async Task AnalyzeItemWithAIAsync(ClothingItem item)
    {
        IsLoading = true;
        try
        {
            var request = new ImageAnalysisRequest
            {
                ClothingItemId = item.ClothingItemId,
                ImageUrl = item.LinkToPhoto
            };
            
            var response = await _apiService.AnalyzeClothingImageAsync(request);
            await Shell.Current.DisplayAlert("AI Analysis", "Image analyzed successfully!", "OK");
            await LoadItemsAsync(); // Reload to get updated tags
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"AI analysis failed: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
