using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyCloset.Mobile.Models;
using Newtonsoft.Json;

namespace MyCloset.Mobile.Services;

public interface IApiService
{
    string BaseUrl { get; set; }
    string AuthToken { get; set; }
    
    // Closet endpoints
    Task<ApiResponse<List<ClothingItem>>> GetClothingItemsAsync();
    Task<ApiResponse<ClothingItem>> AddClothingItemAsync(ClothingItemRequest request);
    Task<ApiResponse<ClothingItem>> UpdateClothingItemAsync(Guid id, ClothingItemRequest request);
    Task<ApiResponse<bool>> DeleteClothingItemAsync(Guid id);
    
    // Outfit endpoints
    Task<ApiResponse<List<Outfit>>> GetOutfitsAsync();
    Task<ApiResponse<Outfit>> CreateOutfitAsync(OutfitRequest request);
    Task<ApiResponse<bool>> DeleteOutfitAsync(Guid id);
    
    // AI endpoints
    Task<ApiResponse<object>> GetOutfitRecommendationsAsync(OutfitRecommendationRequest request);
    Task<ApiResponse<object>> AnalyzeClothingImageAsync(ImageAnalysisRequest request);
    Task<ApiResponse<object>> GetStylingTipsAsync(Guid clothingItemId);
    Task<ApiResponse<object>> IdentifyWardrobeGapsAsync();
    
    // Social Media endpoints
    Task<ApiResponse<List<SocialMediaConnection>>> GetConnectedAccountsAsync();
    Task<ApiResponse<object>> LinkSocialMediaAccountAsync(LinkSocialMediaRequest request);
    Task<ApiResponse<bool>> DisconnectAccountAsync(Guid connectionId);
    Task<ApiResponse<List<OutfitHistory>>> GetOutfitHistoryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<object>> RecordOutfitWornAsync(RecordOutfitWornRequest request);
    Task<ApiResponse<object>> GetOutfitAnalyticsAsync();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public string BaseUrl { get; set; } = "https://localhost:5001/api"; // Update with your backend URL
    public string AuthToken { get; set; } = string.Empty;

    public ApiService()
    {
        _httpClient = new HttpClient();
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string endpoint)
    {
        var request = new HttpRequestMessage(method, $"{BaseUrl}{endpoint}");
        if (!string.IsNullOrEmpty(AuthToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);
        }
        return request;
    }

    private async Task<ApiResponse<T>> SendRequestAsync<T>(HttpRequestMessage request)
    {
        try
        {
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var data = JsonConvert.DeserializeObject<T>(content);
                return new ApiResponse<T>
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "Success",
                    Data = data
                };
            }

            return new ApiResponse<T>
            {
                StatusCode = (int)response.StatusCode,
                Message = content,
                Data = default
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                StatusCode = 500,
                Message = ex.Message,
                Data = default
            };
        }
    }

    // Closet endpoints
    public async Task<ApiResponse<List<ClothingItem>>> GetClothingItemsAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/Closet/GetClothingItems");
        return await SendRequestAsync<List<ClothingItem>>(request);
    }

    public async Task<ApiResponse<ClothingItem>> AddClothingItemAsync(ClothingItemRequest itemRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/Closet/AddClothingItem");
        request.Content = JsonContent.Create(itemRequest);
        return await SendRequestAsync<ClothingItem>(request);
    }

    public async Task<ApiResponse<ClothingItem>> UpdateClothingItemAsync(Guid id, ClothingItemRequest itemRequest)
    {
        var request = CreateRequest(HttpMethod.Put, $"/Closet/UpdateClothingItem/{id}");
        request.Content = JsonContent.Create(itemRequest);
        return await SendRequestAsync<ClothingItem>(request);
    }

    public async Task<ApiResponse<bool>> DeleteClothingItemAsync(Guid id)
    {
        var request = CreateRequest(HttpMethod.Delete, $"/Closet/DeleteClothingItem/{id}");
        return await SendRequestAsync<bool>(request);
    }

    // Outfit endpoints
    public async Task<ApiResponse<List<Outfit>>> GetOutfitsAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/Closet/GetOutfits");
        return await SendRequestAsync<List<Outfit>>(request);
    }

    public async Task<ApiResponse<Outfit>> CreateOutfitAsync(OutfitRequest outfitRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/Closet/CreateOutfit");
        request.Content = JsonContent.Create(outfitRequest);
        return await SendRequestAsync<Outfit>(request);
    }

    public async Task<ApiResponse<bool>> DeleteOutfitAsync(Guid id)
    {
        var request = CreateRequest(HttpMethod.Delete, $"/Closet/DeleteOutfit/{id}");
        return await SendRequestAsync<bool>(request);
    }

    // AI endpoints
    public async Task<ApiResponse<object>> GetOutfitRecommendationsAsync(OutfitRecommendationRequest recommendationRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/AI/GetOutfitRecommendations");
        request.Content = JsonContent.Create(recommendationRequest);
        return await SendRequestAsync<object>(request);
    }

    public async Task<ApiResponse<object>> AnalyzeClothingImageAsync(ImageAnalysisRequest analysisRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/AI/AnalyzeClothingImage");
        request.Content = JsonContent.Create(analysisRequest);
        return await SendRequestAsync<object>(request);
    }

    public async Task<ApiResponse<object>> GetStylingTipsAsync(Guid clothingItemId)
    {
        var request = CreateRequest(HttpMethod.Get, $"/AI/GetStylingTips/{clothingItemId}");
        return await SendRequestAsync<object>(request);
    }

    public async Task<ApiResponse<object>> IdentifyWardrobeGapsAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/AI/IdentifyWardrobeGaps");
        return await SendRequestAsync<object>(request);
    }

    // Social Media endpoints
    public async Task<ApiResponse<List<SocialMediaConnection>>> GetConnectedAccountsAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/SocialMedia/ConnectedAccounts");
        return await SendRequestAsync<List<SocialMediaConnection>>(request);
    }

    public async Task<ApiResponse<object>> LinkSocialMediaAccountAsync(LinkSocialMediaRequest linkRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/SocialMedia/LinkAccount");
        request.Content = JsonContent.Create(linkRequest);
        return await SendRequestAsync<object>(request);
    }

    public async Task<ApiResponse<bool>> DisconnectAccountAsync(Guid connectionId)
    {
        var request = CreateRequest(HttpMethod.Delete, $"/SocialMedia/DisconnectAccount/{connectionId}");
        return await SendRequestAsync<bool>(request);
    }

    public async Task<ApiResponse<List<OutfitHistory>>> GetOutfitHistoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var endpoint = "/SocialMedia/OutfitHistory";
        if (startDate.HasValue && endDate.HasValue)
        {
            endpoint += $"?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        }
        var request = CreateRequest(HttpMethod.Get, endpoint);
        return await SendRequestAsync<List<OutfitHistory>>(request);
    }

    public async Task<ApiResponse<object>> RecordOutfitWornAsync(RecordOutfitWornRequest wornRequest)
    {
        var request = CreateRequest(HttpMethod.Post, "/SocialMedia/RecordOutfitWorn");
        request.Content = JsonContent.Create(wornRequest);
        return await SendRequestAsync<object>(request);
    }

    public async Task<ApiResponse<object>> GetOutfitAnalyticsAsync()
    {
        var request = CreateRequest(HttpMethod.Get, "/SocialMedia/Analytics");
        return await SendRequestAsync<object>(request);
    }
}
