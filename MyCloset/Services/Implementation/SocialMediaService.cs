using System.Net;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Services.Interfaces;
using Newtonsoft.Json;

namespace MyCloset.Services.Implementation;

public class SocialMediaService : ISocialMediaService
{
    private readonly ILogger<SocialMediaService> _logger;
    private readonly MyClosetAppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IAIService _aiService;

    public SocialMediaService(
        ILogger<SocialMediaService> logger,
        MyClosetAppDbContext dbContext,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IBlobStorageService blobStorageService,
        IAIService aiService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();
        _blobStorageService = blobStorageService;
        _aiService = aiService;
    }

    public async Task<ClosetActionResult> LinkSocialMediaAccountAsync(Guid userId, string platform, string accessToken)
    {
        try
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "User not found."
                };
            }

            // In production: Validate access token with platform API and get user profile
            // For now, we'll create a connection record
            
            // Check if connection already exists
            var existingConnection = await _dbContext.Set<SocialMediaConnection>()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Platform.Equals(platform, StringComparison.OrdinalIgnoreCase));

            if (existingConnection != null)
            {
                // Update existing connection
                existingConnection.AccessToken = accessToken; // In production: encrypt this
                existingConnection.TokenExpiresAt = DateTime.UtcNow.AddDays(60); // Platform-specific
                existingConnection.LastSynced = DateTime.UtcNow;
                existingConnection.IsActive = true;
            }
            else
            {
                // Create new connection
                var connection = new SocialMediaConnection
                {
                    UserId = userId,
                    Platform = platform,
                    ExternalUserId = $"{platform}_user_{userId}", // In production: get from API
                    AccessToken = accessToken, // In production: encrypt this
                    TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                    IsActive = true,
                    DateConnected = DateTime.UtcNow,
                    LastSynced = DateTime.UtcNow
                };

                await _dbContext.Set<SocialMediaConnection>().AddAsync(connection);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User {userId} linked {platform} account");

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"{platform} account linked successfully.",
                Data = new { Platform = platform, Status = "Connected" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error linking social media account for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to link social media account."
            };
        }
    }

    public async Task<ClosetActionResult> SyncSocialMediaPostsAsync(Guid userId, string? platform = null)
    {
        try
        {
            // In a real implementation, this would:
            // 1. Use platform APIs (Instagram Graph API, Facebook Graph API, etc.)
            // 2. Fetch recent posts with outfit/fashion tags
            // 3. Download images and analyze them with AI
            // 4. Match posts to existing outfits or create new ones

            // For demonstration, we'll return existing posts
            var query = _dbContext.Set<SocialMediaPost>()
                .Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(platform))
            {
                query = query.Where(x => x.Platform.Equals(platform, StringComparison.OrdinalIgnoreCase));
            }

            var posts = await query
                .OrderByDescending(x => x.PostDate)
                .Take(50)
                .ToListAsync();

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Synced {posts.Count} posts.",
                Data = posts.Select(p => new
                {
                    p.SocialMediaPostId,
                    p.Platform,
                    p.PostUrl,
                    p.PostDate,
                    p.Caption,
                    p.ImageUrl,
                    p.OutfitId
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error syncing social media posts for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to sync social media posts."
            };
        }
    }

    public async Task<ClosetActionResult> AddSocialMediaPostAsync(Guid userId, string postUrl, string platform, Guid? outfitId = null)
    {
        try
        {
            // Verify user exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "User not found."
                };
            }

            // Verify outfit exists if provided
            if (outfitId.HasValue)
            {
                var outfit = await _dbContext.Outfits
                    .FirstOrDefaultAsync(x => x.OutfitId == outfitId.Value && x.UserId == userId);
                
                if (outfit == null)
                {
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Outfit not found."
                    };
                }
            }

            // Create social media post record
            var post = new SocialMediaPost
            {
                UserId = userId,
                OutfitId = outfitId,
                Platform = platform,
                PostUrl = postUrl,
                PostDate = DateTime.UtcNow, // In real implementation, fetch from platform API
                DateAdded = DateTime.UtcNow,
                LastSynced = DateTime.UtcNow
            };

            await _dbContext.Set<SocialMediaPost>().AddAsync(post);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Added social media post for user {userId} from {platform}");

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Social media post added successfully.",
                Data = new
                {
                    post.SocialMediaPostId,
                    post.Platform,
                    post.PostUrl,
                    post.OutfitId
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding social media post for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to add social media post."
            };
        }
    }

    public async Task<ClosetActionResult> GetOutfitHistoryAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var outfits = await _dbContext.Outfits
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // Flatten worn history and filter
            var historyRecords = outfits.SelectMany(outfit => 
                outfit.WornHistory.Select(history => new 
                {
                    Outfit = outfit,
                    History = history
                }))
                .Where(x => (!startDate.HasValue || x.History.DateWorn >= startDate.Value) &&
                           (!endDate.HasValue || x.History.DateWorn <= endDate.Value))
                .OrderByDescending(x => x.History.DateWorn)
                .ToList();

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Retrieved {historyRecords.Count} outfit history records.",
                Data = historyRecords.Select(h => new
                {
                    OutfitId = h.Outfit.OutfitId,
                    h.History.DateWorn,
                    h.History.Location,
                    h.History.Occasion,
                    Notes = h.Outfit.Notes,
                    Outfit = new
                    {
                        h.Outfit.OutfitId,
                        Name = h.Outfit.Name,
                        ItemCount = h.Outfit.ClothingItems.Count,
                        Items = h.Outfit.ClothingItems.Select(i => new
                        {
                            i.ClothingItemId,
                            i.Title,
                            i.Category,
                            i.LinkToPhoto
                        })
                    },
                    // Social media post data would need to be retrieved separately if needed
                    SocialMediaPost = (object?)null
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving outfit history for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve outfit history."
            };
        }
    }

    public async Task<ClosetActionResult> RecordOutfitWornAsync(
        Guid userId,
        Guid outfitId,
        DateTime dateWorn,
        string? location = null,
        string? occasion = null,
        string? notes = null,
        Guid? socialMediaPostId = null)
    {
        try
        {
            // Verify outfit exists and belongs to user
            var outfit = await _dbContext.Outfits
                .FirstOrDefaultAsync(x => x.OutfitId == outfitId && x.UserId == userId);

            if (outfit == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Outfit not found."
                };
            }

            // Verify social media post if provided
            if (socialMediaPostId.HasValue)
            {
                var post = await _dbContext.Set<SocialMediaPost>()
                    .FirstOrDefaultAsync(x => x.SocialMediaPostId == socialMediaPostId.Value && x.UserId == userId);

                if (post == null)
                {
                    return new ClosetActionResult
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = "Social media post not found."
                    };
                }
            }

            // Create outfit history record
            var history = new OutfitHistory
            {
                UserId = userId,
                OutfitId = outfitId,
                DateWorn = dateWorn,
                Location = location,
                Occasion = occasion,
                Notes = notes,
                SocialMediaPostId = socialMediaPostId,
                DateAdded = DateTime.UtcNow
            };

            await _dbContext.Set<OutfitHistory>().AddAsync(history);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Recorded outfit worn for user {userId}, outfit {outfitId}");

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Outfit history recorded successfully.",
                Data = new
                {
                    history.OutfitHistoryId,
                    history.DateWorn,
                    history.Location,
                    history.Occasion
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error recording outfit worn for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to record outfit history."
            };
        }
    }

    public async Task<ClosetActionResult> GetPostsForOutfitAsync(Guid outfitId)
    {
        try
        {
            var posts = await _dbContext.Set<SocialMediaPost>()
                .Where(x => x.OutfitId == outfitId)
                .OrderByDescending(x => x.PostDate)
                .ToListAsync();

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Retrieved {posts.Count} posts for outfit.",
                Data = posts.Select(p => new
                {
                    p.SocialMediaPostId,
                    p.Platform,
                    p.PostUrl,
                    p.PostDate,
                    p.Caption,
                    p.ImageUrl,
                    p.LikesCount,
                    p.CommentsCount
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving posts for outfit {outfitId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve posts."
            };
        }
    }

    public async Task<ClosetActionResult> GetOutfitAnalyticsAsync(Guid userId)
    {
        try
        {
            var outfits = await _dbContext.Outfits
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var posts = await _dbContext.Set<SocialMediaPost>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // Flatten worn history for analytics
            var allWornHistory = outfits.SelectMany(o => 
                o.WornHistory.Select(h => new { Outfit = o, History = h })
            ).ToList();

            // Calculate analytics
            var totalOutfitsWorn = allWornHistory.Count;
            var uniqueOutfits = allWornHistory.Select(x => x.Outfit.OutfitId).Distinct().Count();
            var averageWearFrequency = uniqueOutfits > 0 ? (double)totalOutfitsWorn / uniqueOutfits : 0;

            var mostWornOutfit = allWornHistory
                .GroupBy(x => x.Outfit.OutfitId)
                .OrderByDescending(g => g.Count())
                .Select(g => new
                {
                    OutfitId = g.Key,
                    WearCount = g.Count(),
                    OutfitName = g.First().Outfit.Name
                })
                .FirstOrDefault();

            var occasionBreakdown = allWornHistory
                .Where(x => !string.IsNullOrEmpty(x.History.Occasion))
                .GroupBy(x => x.History.Occasion)
                .Select(g => new
                {
                    Occasion = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            var socialMediaStats = new
            {
                TotalPosts = posts.Count,
                PostsByPlatform = posts.GroupBy(x => x.Platform)
                    .Select(g => new { Platform = g.Key, Count = g.Count() })
                    .ToList(),
                TotalLikes = posts.Sum(x => x.LikesCount ?? 0),
                TotalComments = posts.Sum(x => x.CommentsCount ?? 0)
            };

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Analytics retrieved successfully.",
                Data = new
                {
                    TotalOutfitsWorn = totalOutfitsWorn,
                    UniqueOutfits = uniqueOutfits,
                    AverageWearFrequency = Math.Round(averageWearFrequency, 2),
                    MostWornOutfit = mostWornOutfit,
                    OccasionBreakdown = occasionBreakdown,
                    SocialMediaStats = socialMediaStats
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting analytics for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve analytics."
            };
        }
    }

    public async Task<ClosetActionResult> FindSimilarOutfitsAsync(Guid userId, string imageUrl)
    {
        try
        {
            // Use AI service to analyze the image
            var analysisResult = await _aiService.AnalyzeOutfitImageAsync(imageUrl, userId);

            if (analysisResult.StatusCode != HttpStatusCode.OK)
            {
                return analysisResult;
            }

            // Get user's outfits
            var userOutfits = await _dbContext.Outfits
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // In a production system, you'd use vector similarity or more sophisticated matching
            // For now, we'll do simple category/color matching
            var analysisData = analysisResult.Data as dynamic;
            var matchingOutfits = new List<object>();

            foreach (var outfit in userOutfits)
            {
                var matchScore = CalculateOutfitSimilarityScore(outfit, analysisData);
                if (matchScore > 0.5) // 50% similarity threshold
                {
                    matchingOutfits.Add(new
                    {
                        outfit.OutfitId,
                        Name = outfit.Name,
                        outfit.Tags,
                        ItemCount = outfit.ClothingItems.Count,
                        SimilarityScore = Math.Round(matchScore * 100, 0),
                        Items = outfit.ClothingItems.Select(i => new
                        {
                            i.ClothingItemId,
                            i.Title,
                            i.Category,
                            i.LinkToPhoto
                        })
                    });
                }
            }

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Found {matchingOutfits.Count} similar outfits.",
                Data = matchingOutfits.OrderByDescending(x => ((dynamic)x).SimilarityScore)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finding similar outfits for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to find similar outfits."
            };
        }
    }

    private double CalculateOutfitSimilarityScore(Outfit outfit, dynamic analysisData)
    {
        // Simple similarity calculation based on categories
        // In production, you'd use more sophisticated algorithms
        try
        {
            if (analysisData?.MatchingItemsInCloset == null)
                return 0;

            var matchingCategories = new HashSet<string>();
            foreach (var item in analysisData.MatchingItemsInCloset)
            {
                string category = item.Category?.ToString() ?? "";
                matchingCategories.Add(category);
            }

            var outfitCategories = outfit.ClothingItems.Select(x => x.Category).ToHashSet();
            var intersection = matchingCategories.Intersect(outfitCategories).Count();
            var union = matchingCategories.Union(outfitCategories).Count();

            return union > 0 ? (double)intersection / union : 0;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<ClosetActionResult> GetConnectedAccountsAsync(Guid userId)
    {
        try
        {
            var connections = await _dbContext.Set<SocialMediaConnection>()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateConnected)
                .ToListAsync();

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"Retrieved {connections.Count} connected accounts.",
                Data = connections.Select(c => new
                {
                    c.ConnectionId,
                    c.Platform,
                    c.Username,
                    c.ProfileUrl,
                    c.IsActive,
                    c.DateConnected,
                    c.LastSynced,
                    TokenExpired = c.TokenExpiresAt.HasValue && c.TokenExpiresAt.Value < DateTime.UtcNow
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting connected accounts for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to retrieve connected accounts."
            };
        }
    }

    public async Task<ClosetActionResult> DisconnectAccountAsync(Guid userId, Guid connectionId)
    {
        try
        {
            var connection = await _dbContext.Set<SocialMediaConnection>()
                .FirstOrDefaultAsync(x => x.ConnectionId == connectionId && x.UserId == userId);

            if (connection == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Connection not found."
                };
            }

            connection.IsActive = false;
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"User {userId} disconnected {connection.Platform} account");

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = $"{connection.Platform} account disconnected successfully."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error disconnecting account for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to disconnect account."
            };
        }
    }
}
