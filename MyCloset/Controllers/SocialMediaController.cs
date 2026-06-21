using Microsoft.AspNetCore.Mvc;
using MyCloset.Models;
using MyCloset.Models.RequestModels;
using MyCloset.Services.Interfaces;

namespace MyCloset.Controllers;

/// <summary>
/// Social media integration and outfit history tracking
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SocialMediaController : BaseController
{
    private readonly ILogger<SocialMediaController> _logger;
    private readonly ISocialMediaService _socialMediaService;

    public SocialMediaController(ILogger<SocialMediaController> logger, ISocialMediaService socialMediaService)
    {
        _logger = logger;
        _socialMediaService = socialMediaService;
    }

    /// <summary>
    /// Link a social media account
    /// </summary>
    /// <param name="request">Social media account linking request</param>
    /// <returns>Success result</returns>
    [HttpPost("LinkAccount")]
    public async Task<IActionResult> LinkAccount([FromBody] LinkSocialMediaRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} linking {request.Platform} account");

            var result = await _socialMediaService.LinkSocialMediaAccountAsync(
                currentUser,
                request.Platform,
                request.AccessToken
            );

            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error linking social media account: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to link social media account."
            });
        }
    }

    /// <summary>
    /// Sync posts from linked social media accounts
    /// </summary>
    /// <param name="platform">Optional platform filter</param>
    /// <returns>List of synced posts</returns>
    [HttpPost("SyncPosts")]
    public async Task<IActionResult> SyncPosts([FromQuery] string? platform = null)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} syncing social media posts");

            var result = await _socialMediaService.SyncSocialMediaPostsAsync(currentUser, platform);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error syncing social media posts: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to sync social media posts."
            });
        }
    }

    /// <summary>
    /// Manually add a social media post
    /// </summary>
    /// <param name="request">Add post request</param>
    /// <returns>Created social media post</returns>
    [HttpPost("AddPost")]
    public async Task<IActionResult> AddPost([FromBody] AddSocialMediaPostRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} adding social media post from {request.Platform}");

            var result = await _socialMediaService.AddSocialMediaPostAsync(
                currentUser,
                request.PostUrl,
                request.Platform,
                request.OutfitId
            );

            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding social media post: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to add social media post."
            });
        }
    }

    /// <summary>
    /// Get outfit history for the current user
    /// </summary>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>List of outfit history records</returns>
    [HttpGet("OutfitHistory")]
    public async Task<IActionResult> GetOutfitHistory([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} retrieving outfit history");

            var result = await _socialMediaService.GetOutfitHistoryAsync(currentUser, startDate, endDate);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving outfit history: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve outfit history."
            });
        }
    }

    /// <summary>
    /// Record when an outfit was worn
    /// </summary>
    /// <param name="request">Record outfit worn request</param>
    /// <returns>Created outfit history record</returns>
    [HttpPost("RecordOutfitWorn")]
    public async Task<IActionResult> RecordOutfitWorn([FromBody] RecordOutfitWornRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} recording outfit worn");

            var result = await _socialMediaService.RecordOutfitWornAsync(
                currentUser,
                request.OutfitId,
                request.DateWorn,
                request.Location,
                request.Occasion,
                request.Notes,
                request.SocialMediaPostId
            );

            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording outfit worn: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to record outfit worn."
            });
        }
    }

    /// <summary>
    /// Get posts featuring a specific outfit
    /// </summary>
    /// <param name="outfitId">Outfit ID</param>
    /// <returns>List of social media posts</returns>
    [HttpGet("PostsForOutfit/{outfitId}")]
    public async Task<IActionResult> GetPostsForOutfit(Guid outfitId)
    {
        try
        {
            _logger.LogInformation($"Retrieving posts for outfit {outfitId}");

            var result = await _socialMediaService.GetPostsForOutfitAsync(outfitId);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving posts for outfit: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve posts."
            });
        }
    }

    /// <summary>
    /// Get outfit analytics (wearing patterns, most worn outfits, etc.)
    /// </summary>
    /// <returns>Analytics data</returns>
    [HttpGet("Analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} retrieving outfit analytics");

            var result = await _socialMediaService.GetOutfitAnalyticsAsync(currentUser);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving analytics: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve analytics."
            });
        }
    }

    /// <summary>
    /// Find similar outfits in closet based on an image
    /// </summary>
    /// <param name="request">Find similar outfits request</param>
    /// <returns>Similar outfits from user's closet</returns>
    [HttpPost("FindSimilarOutfits")]
    public async Task<IActionResult> FindSimilarOutfits([FromBody] FindSimilarOutfitsRequest request)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} finding similar outfits");

            var result = await _socialMediaService.FindSimilarOutfitsAsync(currentUser, request.ImageUrl);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finding similar outfits: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to find similar outfits."
            });
        }
    }

    /// <summary>
    /// Get connected social media accounts
    /// </summary>
    /// <returns>List of connected accounts</returns>
    [HttpGet("ConnectedAccounts")]
    public async Task<IActionResult> GetConnectedAccounts()
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} retrieving connected accounts");

            var result = await _socialMediaService.GetConnectedAccountsAsync(currentUser);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving connected accounts: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve connected accounts."
            });
        }
    }

    /// <summary>
    /// Disconnect a social media account
    /// </summary>
    /// <param name="connectionId">Connection ID to disconnect</param>
    /// <returns>Success result</returns>
    [HttpDelete("DisconnectAccount/{connectionId}")]
    public async Task<IActionResult> DisconnectAccount(Guid connectionId)
    {
        try
        {
            Guid currentUser = await GetCurrentUserGuid();
            _logger.LogInformation($"User {currentUser} disconnecting account {connectionId}");

            var result = await _socialMediaService.DisconnectAccountAsync(currentUser, connectionId);
            return ResultHelper(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error disconnecting account: {ex.Message}");
            return ResultHelper(new ClosetActionResult
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Message = "Failed to disconnect account."
            });
        }
    }
}
