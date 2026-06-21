using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyCloset.Mobile.Models;
using MyCloset.Mobile.Services;

namespace MyCloset.Mobile.ViewModels;

public partial class AIRecommendationsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<OutfitRecommendation> recommendations = new();

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string selectedOccasion = "Casual";

    [ObservableProperty]
    private string selectedWeather = "Sunny";

    [ObservableProperty]
    private string selectedSeason = "Spring";

    public ObservableCollection<string> Occasions { get; } = new()
    {
        "Casual", "Business", "Formal", "Date", "Workout", "Party"
    };

    public ObservableCollection<string> WeatherOptions { get; } = new()
    {
        "Sunny", "Rainy", "Cold", "Warm", "Hot"
    };

    public ObservableCollection<string> Seasons { get; } = new()
    {
        "Spring", "Summer", "Fall", "Winter"
    };

    public AIRecommendationsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task GetRecommendationsAsync()
    {
        IsLoading = true;
        try
        {
            var request = new OutfitRecommendationRequest
            {
                Occasion = SelectedOccasion,
                Weather = SelectedWeather,
                Season = SelectedSeason
            };

            var response = await _apiService.GetOutfitRecommendationsAsync(request);
            // Parse recommendations from response.Data
            // For now, showing success message
            await Shell.Current.DisplayAlert("Success", "AI recommendations generated!", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to get recommendations: {ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task IdentifyWardrobeGapsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _apiService.IdentifyWardrobeGapsAsync();
            await Shell.Current.DisplayAlert("Wardrobe Gaps", "Analysis complete!", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
