using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Models.RequestModels;
using MyCloset.Services.Interfaces;

namespace MyCloset.Controllers;

/// <summary>
/// AI-powered outfit recommendations and image analysis
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AIController : BaseController
{
    private readonly ILogger<AIController> _logger;
    private readonly IAIService _aiService;
    private readonly IMyClosetService _closetService;

    public AIController(ILogger<AIController> logger, IAIService aiService, IMyClosetService closetService)
    {
        _logger = logger;
        _aiService = aiService;
        _closetService = closetService;
    }

    /// <summary>
    /// Analyze a clothing item image with AI
    /// </summary>
    /// <param name="request">Image analysis request</param>
    /// <returns>AI analysis results including tags, colors, patterns</returns>
    [HttpPost("AnalyzeClothingImage")]
    public async Task<IActionResult> AnalyzeClothingImage([FromBody] ImageAnalysisRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} analyzing clothing image for item {request.ClothingItemId}");

            var result = await _aiService.AnalyzeClothingImageAsync(request.ImageUrl, request.ClothingItemId);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error analyzing clothing image: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to analyze clothing image."
            });
        }
    }

    /// <summary>
    /// Generate outfit recommendations based on occasion and preferences
    /// </summary>
    /// <param name="request">Outfit recommendation request</param>
    /// <returns>List of recommended outfits</returns>
    [HttpPost("GetOutfitRecommendations")]
    public async Task<IActionResult> GetOutfitRecommendations([FromBody] OutfitRecommendationRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} requesting outfit recommendations for {request.Occasion}");

            var result = await _aiService.GenerateOutfitRecommendationsAsync(
                currentUser,
                request.Occasion,
                request.Weather,
                request.Season,
                request.ExcludeItemIds
            );

            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating outfit recommendations: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to generate outfit recommendations."
            });
        }
    }

    /// <summary>
    /// Get AI suggestions to complete a partially selected outfit
    /// </summary>
    /// <param name="request">Completion suggestion request</param>
    /// <returns>Suggested items to complete the outfit</returns>
    [HttpPost("GetOutfitCompletionSuggestions")]
    public async Task<IActionResult> GetOutfitCompletionSuggestions([FromBody] OutfitCompletionRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} requesting completion suggestions for {request.SelectedItemIds.Count} items");

            var result = await _aiService.GetOutfitCompletionSuggestionsAsync(
                currentUser,
                request.SelectedItemIds,
                request.Occasion
            );

            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting completion suggestions: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to get completion suggestions."
            });
        }
    }

    /// <summary>
    /// Analyze a full outfit image (e.g., from social media)
    /// </summary>
    /// <param name="request">Outfit image analysis request</param>
    /// <returns>Detected items, style, and matching items in closet</returns>
    [HttpPost("AnalyzeOutfitImage")]
    public async Task<IActionResult> AnalyzeOutfitImage([FromBody] OutfitImageAnalysisRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} analyzing outfit image");

            var result = await _aiService.AnalyzeOutfitImageAsync(request.ImageUrl, currentUser);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error analyzing outfit image: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to analyze outfit image."
            });
        }
    }

    /// <summary>
    /// Get styling tips for a specific clothing item
    /// </summary>
    /// <param name="clothingItemId">Clothing item ID</param>
    /// <returns>Styling tips and pairing suggestions</returns>
    [HttpGet("GetStylingTips/{clothingItemId}")]
    public async Task<IActionResult> GetStylingTips(Guid clothingItemId)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} requesting styling tips for item {clothingItemId}");

            var result = await _aiService.GetStylingTipsAsync(clothingItemId, currentUser);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting styling tips: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to get styling tips."
            });
        }
    }

    /// <summary>
    /// Identify missing wardrobe essentials
    /// </summary>
    /// <returns>List of suggested items to add to wardrobe</returns>
    [HttpGet("IdentifyWardrobeGaps")]
    public async Task<IActionResult> IdentifyWardrobeGaps()
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} requesting wardrobe gap analysis");

            var result = await _aiService.IdentifyWardrobeGapsAsync(currentUser);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error identifying wardrobe gaps: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to identify wardrobe gaps."
            });
        }
    }
}
