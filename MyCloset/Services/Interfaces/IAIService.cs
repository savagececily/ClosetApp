using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Models.RequestModels;

namespace MyCloset.Services.Interfaces;

/// <summary>
/// AI service for outfit recommendations and image analysis
/// </summary>
public interface IAIService
{
    /// <summary>
    /// Analyze a clothing item image and generate tags, colors, patterns
    /// </summary>
    /// <param name="imageUrl">URL or base64 of the image</param>
    /// <param name="clothingItemId">ID of the clothing item</param>
    /// <returns>AI analysis result</returns>
    Task<ClosetActionResult> AnalyzeClothingImageAsync(string imageUrl, Guid clothingItemId);

    /// <summary>
    /// Generate outfit recommendations based on user's closet, occasion, and preferences
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="occasion">Occasion type (casual, business, formal, date, etc.)</param>
    /// <param name="weather">Weather conditions (optional)</param>
    /// <param name="season">Season (optional)</param>
    /// <param name="excludeItemIds">Items to exclude from recommendations</param>
    /// <returns>List of outfit recommendations</returns>
    Task<ClosetActionResult> GenerateOutfitRecommendationsAsync(Guid userId, string occasion, string? weather = null, string? season = null, List<Guid>? excludeItemIds = null);

    /// <summary>
    /// Get AI suggestions for completing an outfit (user has selected some items)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="selectedItemIds">Items already selected by user</param>
    /// <param name="occasion">Occasion type</param>
    /// <returns>Suggestions for completing the outfit</returns>
    Task<ClosetActionResult> GetOutfitCompletionSuggestionsAsync(Guid userId, List<Guid> selectedItemIds, string occasion);

    /// <summary>
    /// Analyze a full outfit image from social media or upload
    /// </summary>
    /// <param name="imageUrl">URL or base64 of the outfit image</param>
    /// <param name="userId">User ID</param>
    /// <returns>Analysis including detected items and style suggestions</returns>
    Task<ClosetActionResult> AnalyzeOutfitImageAsync(string imageUrl, Guid userId);

    /// <summary>
    /// Get styling tips for a specific clothing item
    /// </summary>
    /// <param name="clothingItemId">Clothing item ID</param>
    /// <param name="userId">User ID</param>
    /// <returns>Styling tips and matching suggestions</returns>
    Task<ClosetActionResult> GetStylingTipsAsync(Guid clothingItemId, Guid userId);

    /// <summary>
    /// Identify missing wardrobe essentials based on user's closet
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of suggested items to add</returns>
    Task<ClosetActionResult> IdentifyWardrobeGapsAsync(Guid userId);
}
