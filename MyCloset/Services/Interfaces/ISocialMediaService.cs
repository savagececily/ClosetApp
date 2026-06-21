using MyCloset.Models;
using MyCloset.Models.DBModels;

namespace MyCloset.Services.Interfaces;

/// <summary>
/// Service for social media integration and outfit history tracking
/// </summary>
public interface ISocialMediaService
{
    /// <summary>
    /// Link a social media account for tracking posts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="platform">Social media platform (Instagram, Facebook, etc.)</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <returns>Success result</returns>
    Task<ClosetActionResult> LinkSocialMediaAccountAsync(Guid userId, string platform, string accessToken);

    /// <summary>
    /// Sync posts from linked social media accounts
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="platform">Optional platform filter</param>
    /// <returns>List of synced posts</returns>
    Task<ClosetActionResult> SyncSocialMediaPostsAsync(Guid userId, string? platform = null);

    /// <summary>
    /// Manually add a social media post
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="postUrl">URL to the social media post</param>
    /// <param name="platform">Platform name</param>
    /// <param name="outfitId">Optional outfit ID if already exists</param>
    /// <returns>Created social media post</returns>
    Task<ClosetActionResult> AddSocialMediaPostAsync(Guid userId, string postUrl, string platform, Guid? outfitId = null);

    /// <summary>
    /// Get outfit history for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>List of outfit history records</returns>
    Task<ClosetActionResult> GetOutfitHistoryAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// Record when an outfit was worn
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="outfitId">Outfit ID</param>
    /// <param name="dateWorn">Date outfit was worn</param>
    /// <param name="location">Optional location</param>
    /// <param name="occasion">Optional occasion</param>
    /// <param name="notes">Optional notes</param>
    /// <param name="socialMediaPostId">Optional linked social media post</param>
    /// <returns>Created outfit history record</returns>
    Task<ClosetActionResult> RecordOutfitWornAsync(
        Guid userId, 
        Guid outfitId, 
        DateTime dateWorn, 
        string? location = null, 
        string? occasion = null, 
        string? notes = null,
        Guid? socialMediaPostId = null);

    /// <summary>
    /// Get posts featuring a specific outfit
    /// </summary>
    /// <param name="outfitId">Outfit ID</param>
    /// <returns>List of social media posts</returns>
    Task<ClosetActionResult> GetPostsForOutfitAsync(Guid outfitId);

    /// <summary>
    /// Get analytics on outfit wearing patterns
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Analytics data</returns>
    Task<ClosetActionResult> GetOutfitAnalyticsAsync(Guid userId);

    /// <summary>
    /// Find outfits similar to a social media post
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="imageUrl">Image URL from social media</param>
    /// <returns>Similar outfits from user's closet</returns>
    Task<ClosetActionResult> FindSimilarOutfitsAsync(Guid userId, string imageUrl);

    /// <summary>
    /// Get all connected social media accounts for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of connected accounts</returns>
    Task<ClosetActionResult> GetConnectedAccountsAsync(Guid userId);

    /// <summary>
    /// Disconnect a social media account
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="connectionId">Connection ID to disconnect</param>
    /// <returns>Success result</returns>
    Task<ClosetActionResult> DisconnectAccountAsync(Guid userId, Guid connectionId);
}
