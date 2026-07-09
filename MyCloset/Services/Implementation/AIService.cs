using System.Net;
using System.Text;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.EntityFrameworkCore;
using MyCloset.Models;
using MyCloset.Models.DBModels;
using MyCloset.Models.ResponseModels;
using MyCloset.Services.Interfaces;
using Newtonsoft.Json;
using OpenAI.Chat;

namespace MyCloset.Services.Implementation;

public class AIService : IAIService
{
    private readonly ILogger<AIService> _logger;
    private readonly MyClosetAppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly AzureOpenAIClient? _aiClient;
    private readonly string _modelDeploymentName;

    public AIService(
        ILogger<AIService> logger,
        MyClosetAppDbContext dbContext,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _dbContext = dbContext;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();

        // Initialize Azure AI Foundry client
        var foundryEndpoint = configuration["AzureAIFoundry:Endpoint"];
        var foundryKey = configuration["AzureAIFoundry:Key"];
        _modelDeploymentName = configuration["AzureAIFoundry:ModelDeploymentName"] ?? "gpt-4";

        if (!string.IsNullOrEmpty(foundryEndpoint) && !string.IsNullOrEmpty(foundryKey))
        {
            _aiClient = new AzureOpenAIClient(new Uri(foundryEndpoint), new AzureKeyCredential(foundryKey));
        }
    }

    public async Task<ClosetActionResult> AnalyzeClothingImageAsync(string imageUrl, Guid clothingItemId)
    {
        try
        {
            var clothingItem = await _dbContext.ClothingItems
                .FirstOrDefaultAsync(x => x.ClothingItemId == clothingItemId);

            if (clothingItem == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Clothing item not found."
                };
            }

            // Analyze image using Azure Computer Vision
            var visionAnalysis = await AnalyzeImageWithVisionAsync(imageUrl);

            // Generate detailed description and tags using GPT-4 Vision
            var gptAnalysis = await AnalyzeImageWithGPT4VisionAsync(imageUrl, clothingItem.Category);

            // Update embedded AI analysis in the clothing item
            clothingItem.AIAnalysis = new EmbeddedAIAnalysis
            {
                AnalyzedAt = DateTime.UtcNow,
                DetectedColors = gptAnalysis.Colors,
                DetectedPatterns = gptAnalysis.Pattern != null ? new List<string> { gptAnalysis.Pattern } : new List<string>(),
                SuggestedTags = gptAnalysis.Tags,
                StylingTips = gptAnalysis.Description,
                Confidence = gptAnalysis.ConfidenceScore
            };

            await _dbContext.SaveChangesAsync();

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Image analysis completed successfully.",
                Data = new
                {
                    Tags = gptAnalysis.Tags,
                    Colors = gptAnalysis.Colors,
                    Pattern = gptAnalysis.Pattern,
                    Description = gptAnalysis.Description,
                    StyleTags = gptAnalysis.StyleTags,
                    SuggestedCategory = gptAnalysis.SuggestedCategory
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error analyzing image for clothing item {clothingItemId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to analyze image."
            };
        }
    }

    public async Task<ClosetActionResult> GenerateOutfitRecommendationsAsync(
        Guid userId, 
        string occasion, 
        string? weather = null, 
        string? season = null, 
        List<Guid>? excludeItemIds = null)
    {
        try
        {
            // Get user's clothing items with AI analysis
            var clothingItems = await _dbContext.ClothingItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (excludeItemIds != null && excludeItemIds.Any())
            {
                clothingItems = clothingItems.Where(x => !excludeItemIds.Contains(x.ClothingItemId)).ToList();
            }

            if (!clothingItems.Any())
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "No clothing items found in closet."
                };
            }

            // Get recent outfits to avoid suggesting recently worn combinations
            var recentOutfits = await _dbContext.Outfits
                .Where(x => x.UserId == userId)
                .Where(x => x.WornHistory.Any(h => h.DateWorn >= DateTime.UtcNow.AddDays(-30)))
                .ToListAsync();

            // Build prompt for GPT
            var prompt = BuildOutfitRecommendationPrompt(clothingItems, occasion, weather, season, recentOutfits);

            // Call OpenAI to generate recommendations
            var recommendations = await GenerateRecommendationsWithGPTAsync(prompt, userId, clothingItems);

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Outfit recommendations generated successfully.",
                Data = recommendations
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating outfit recommendations for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to generate outfit recommendations."
            };
        }
    }

    public async Task<ClosetActionResult> GetOutfitCompletionSuggestionsAsync(
        Guid userId, 
        List<Guid> selectedItemIds, 
        string occasion)
    {
        try
        {
            // Get selected items
            var selectedItems = await _dbContext.ClothingItems
                .Where(x => selectedItemIds.Contains(x.ClothingItemId))
                .ToListAsync();

            // Get available items (excluding selected)
            var availableItems = await _dbContext.ClothingItems
                .Where(x => x.UserId == userId && !selectedItemIds.Contains(x.ClothingItemId))
                .ToListAsync();

            if (!availableItems.Any())
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "No additional items available for suggestions."
                };
            }

            // Build prompt for completion suggestions
            var prompt = BuildCompletionSuggestionPrompt(selectedItems, availableItems, occasion);

            // Call OpenAI
            var suggestions = await GetCompletionSuggestionsFromGPTAsync(prompt, availableItems);

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Outfit completion suggestions generated successfully.",
                Data = suggestions
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating completion suggestions for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to generate completion suggestions."
            };
        }
    }

    public async Task<ClosetActionResult> AnalyzeOutfitImageAsync(string imageUrl, Guid userId)
    {
        try
        {
            // Use GPT-4 Vision to analyze the full outfit
            var analysis = await AnalyzeFullOutfitWithGPT4VisionAsync(imageUrl);

            // Get user's closet to find matching items
            var userItems = await _dbContext.ClothingItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // Find matching items in user's closet
            var matchingItems = FindMatchingItemsInCloset(analysis, userItems);
            
            // Get categories of matching items for filtering
            var matchedCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in matchingItems)
            {
                var category = ((dynamic)item).Category?.ToString();
                if (!string.IsNullOrEmpty(category))
                {
                    matchedCategories.Add(category);
                }
            }

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Outfit image analyzed successfully.",
                Data = new
                {
                    DetectedItems = analysis.DetectedItems,
                    Style = analysis.Style,
                    Occasion = analysis.Occasion,
                    MatchingItemsInCloset = matchingItems,
                    MissingItems = analysis.DetectedItems.Where(x => 
                    {
                        var category = x.category?.ToString() ?? "";
                        return !matchedCategories.Contains(category);
                    }).ToList()
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error analyzing outfit image for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to analyze outfit image."
            };
        }
    }

    public async Task<ClosetActionResult> GetStylingTipsAsync(Guid clothingItemId, Guid userId)
    {
        try
        {
            var clothingItem = await _dbContext.ClothingItems
                .FirstOrDefaultAsync(x => x.ClothingItemId == clothingItemId && x.UserId == userId);

            if (clothingItem == null)
            {
                return new ClosetActionResult
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Clothing item not found."
                };
            }

            // Get other items in closet for pairing suggestions
            var otherItems = await _dbContext.ClothingItems
                .Where(x => x.UserId == userId && x.ClothingItemId != clothingItemId)
                .ToListAsync();

            var prompt = BuildStylingTipsPrompt(clothingItem, otherItems);
            var tips = await GetStylingTipsFromGPTAsync(prompt);

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Styling tips generated successfully.",
                Data = tips
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating styling tips for item {clothingItemId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to generate styling tips."
            };
        }
    }

    public async Task<ClosetActionResult> IdentifyWardrobeGapsAsync(Guid userId)
    {
        try
        {
            var clothingItems = await _dbContext.ClothingItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var prompt = BuildWardrobeGapPrompt(clothingItems);
            var gaps = await IdentifyGapsWithGPTAsync(prompt);

            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Wardrobe analysis completed successfully.",
                Data = gaps
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error identifying wardrobe gaps for user {userId}: {ex.Message}");
            return new ClosetActionResult
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Failed to identify wardrobe gaps."
            };
        }
    }

    #region Private Helper Methods

    private async Task<dynamic> AnalyzeImageWithVisionAsync(string imageUrl)
    {
        // Placeholder for Azure Computer Vision API call
        // This would use Azure Computer Vision SDK to analyze the image
        await Task.CompletedTask;
        return new { Tags = new List<string>(), Colors = new List<string>() };
    }

    private async Task<(List<string> Tags, List<string> Colors, string Pattern, string SuggestedCategory, string Description, List<string> StyleTags, int ConfidenceScore)> 
        AnalyzeImageWithGPT4VisionAsync(string imageUrl, string userCategory)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a fashion expert AI that analyzes clothing images. Provide detailed analysis in JSON format."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart($"Analyze this clothing item (user says it's a {userCategory}). Provide: tags (array), colors (array), pattern (string), suggested_category (string), description (string), style_tags (array like 'casual', 'formal', etc.), confidence_score (0-100)."),
                    ChatMessageContentPart.CreateImagePart(new Uri(imageUrl))
                )
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 800
            });

            var content = chatCompletion.Value.Content[0].Text;

            // Parse JSON response
            var analysis = JsonConvert.DeserializeObject<dynamic>(content);

            return (
                Tags: analysis.tags?.ToObject<List<string>>() ?? new List<string>(),
                Colors: analysis.colors?.ToObject<List<string>>() ?? new List<string>(),
                Pattern: analysis.pattern?.ToString() ?? "",
                SuggestedCategory: analysis.suggested_category?.ToString() ?? userCategory,
                Description: analysis.description?.ToString() ?? "",
                StyleTags: analysis.style_tags?.ToObject<List<string>>() ?? new List<string>(),
                ConfidenceScore: analysis.confidence_score?.ToObject<int>() ?? 75
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GPT-4 Vision");
            // Return default values
            return (new List<string>(), new List<string>(), "", userCategory, "", new List<string>(), 0);
        }
    }

    private string BuildOutfitRecommendationPrompt(
        List<ClothingItem> clothingItems,
        string occasion,
        string? weather,
        string? season,
        List<Outfit> recentOutfits)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are a professional fashion stylist. Create {Math.Min(5, clothingItems.Count / 3)} complete outfit recommendations for the following occasion: {occasion}");
        
        if (!string.IsNullOrEmpty(weather))
            sb.AppendLine($"Weather: {weather}");
        if (!string.IsNullOrEmpty(season))
            sb.AppendLine($"Season: {season}");

        sb.AppendLine("\nAvailable clothing items:");
        foreach (var item in clothingItems)
        {
            sb.AppendLine($"- ID: {item.ClothingItemId}, Title: {item.Title}, Category: {item.Category}, Tags: {string.Join(", ", item.Tags ?? new List<string>())}");
            if (item.AIAnalysis != null)
            {
                sb.AppendLine($"  Colors: {string.Join(", ", item.AIAnalysis.DetectedColors ?? new List<string>())}, Patterns: {string.Join(", ", item.AIAnalysis.DetectedPatterns ?? new List<string>())}");
            }
        }

        if (recentOutfits.Any())
        {
            sb.AppendLine("\nRecently worn combinations (try to suggest different combinations):");
            foreach (var outfit in recentOutfits.Take(5))
            {
                var recentWear = outfit.WornHistory.OrderByDescending(h => h.DateWorn).FirstOrDefault();
                if (recentWear != null)
                {
                    var itemTitles = string.Join(", ", outfit.ClothingItems.Select(x => x.Title));
                    sb.AppendLine($"- {itemTitles} (worn {recentWear.DateWorn:yyyy-MM-dd})");
                }
            }
        }

        sb.AppendLine("\nProvide recommendations in JSON format with: outfit_number, clothing_item_ids (array of GUIDs), reasoning, confidence_score (0-100)");

        return sb.ToString();
    }

    private async Task<List<object>> GenerateRecommendationsWithGPTAsync(string prompt, Guid userId, List<ClothingItem> clothingItems)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a professional fashion stylist AI. Provide outfit recommendations in valid JSON format."),
                new UserChatMessage(prompt)
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.8f,
                MaxOutputTokenCount = 2000
            });

            var content = chatCompletion.Value.Content[0].Text;

            // Parse recommendations
            var recommendations = JsonConvert.DeserializeObject<List<dynamic>>(content);
            var results = new List<object>();

            foreach (var rec in recommendations)
            {
                var itemIds = rec.clothing_item_ids.ToObject<List<Guid>>();
                var items = clothingItems.Where(x => itemIds.Contains(x.ClothingItemId)).ToList();

                results.Add(new
                {
                    RecommendationId = Guid.NewGuid(),
                    Items = items.Select(x => new
                    {
                        x.ClothingItemId,
                        x.Title,
                        x.Category,
                        x.LinkToPhoto
                    }),
                    Reasoning = rec.reasoning?.ToString() ?? "",
                    ConfidenceScore = rec.confidence_score?.ToObject<int>() ?? 75
                });

                // Save to database
                var dbRec = new OutfitRecommendation
                {
                    id = Guid.NewGuid().ToString(),
                    RecommendationId = Guid.NewGuid(),
                    UserId = userId,
                    Occasion = rec.occasion?.ToString() ?? "General",
                    Weather = null,
                    Season = null,
                    Reasoning = rec.reasoning?.ToString() ?? "",
                    ConfidenceScore = rec.confidence_score?.ToObject<int>() ?? 75,
                    RecommendedItems = items.Select(x => new RecommendedClothingItem
                    {
                        ClothingItemId = x.ClothingItemId,
                        Title = x.Title,
                        Category = x.Category,
                        LinkToPhoto = x.LinkToPhoto
                    }).ToList(),
                    DateGenerated = DateTime.UtcNow
                };

                await _dbContext.Set<OutfitRecommendation>().AddAsync(dbRec);
            }

            await _dbContext.SaveChangesAsync();
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations with GPT");
            return new List<object>();
        }
    }

    private string BuildCompletionSuggestionPrompt(List<ClothingItem> selectedItems, List<ClothingItem> availableItems, string occasion)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are a fashion stylist. The user has selected these items for a {occasion} outfit:");
        
        foreach (var item in selectedItems)
        {
            sb.AppendLine($"- {item.Title} ({item.Category})");
        }

        sb.AppendLine("\nSuggest 3-5 additional items from their closet to complete the outfit:");
        foreach (var item in availableItems)
        {
            sb.AppendLine($"- ID: {item.ClothingItemId}, Title: {item.Title}, Category: {item.Category}");
        }

        sb.AppendLine("\nProvide suggestions in JSON format: item_id, reasoning");

        return sb.ToString();
    }

    private async Task<List<object>> GetCompletionSuggestionsFromGPTAsync(string prompt, List<ClothingItem> availableItems)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a fashion stylist AI. Provide outfit completion suggestions in valid JSON format."),
                new UserChatMessage(prompt)
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 1000
            });

            var content = chatCompletion.Value.Content[0].Text;

            var suggestions = JsonConvert.DeserializeObject<List<dynamic>>(content);
            var results = new List<object>();

            foreach (var suggestion in suggestions)
            {
                var itemId = Guid.Parse(suggestion.item_id.ToString());
                var item = availableItems.FirstOrDefault(x => x.ClothingItemId == itemId);
                
                if (item != null)
                {
                    results.Add(new
                    {
                        item.ClothingItemId,
                        item.Title,
                        item.Category,
                        item.LinkToPhoto,
                        Reasoning = suggestion.reasoning?.ToString() ?? ""
                    });
                }
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting completion suggestions from GPT");
            return new List<object>();
        }
    }

    private async Task<(List<dynamic> DetectedItems, string Style, string Occasion)> AnalyzeFullOutfitWithGPT4VisionAsync(string imageUrl)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a fashion expert AI. Analyze outfit images and identify individual clothing items."),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart("Analyze this outfit image. Identify: detected_items (array with category, color, description for each), overall_style, suitable_occasion. Return as JSON."),
                    ChatMessageContentPart.CreateImagePart(new Uri(imageUrl))
                )
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 1000
            });

            var content = chatCompletion.Value.Content[0].Text;

            var analysis = JsonConvert.DeserializeObject<dynamic>(content);

            return (
                DetectedItems: analysis.detected_items?.ToObject<List<dynamic>>() ?? new List<dynamic>(),
                Style: analysis.overall_style?.ToString() ?? "",
                Occasion: analysis.suitable_occasion?.ToString() ?? ""
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing outfit with GPT-4 Vision");
            return (new List<dynamic>(), "", "");
        }
    }

    private List<object> FindMatchingItemsInCloset(
        (List<dynamic> DetectedItems, string Style, string Occasion) analysis,
        List<ClothingItem> userItems)
    {
        var matchingItems = new List<object>();

        foreach (var detectedItem in analysis.DetectedItems)
        {
            string category = detectedItem.category?.ToString() ?? "";
            string color = detectedItem.color?.ToString() ?? "";

            var matches = userItems.Where(x => 
                x.Category.Equals(category, StringComparison.OrdinalIgnoreCase) &&
                (x.AIAnalysis == null || x.AIAnalysis.DetectedColors.Any(c => c.Contains(color, StringComparison.OrdinalIgnoreCase)))
            ).Take(3);

            foreach (var match in matches)
            {
                matchingItems.Add(new
                {
                    match.ClothingItemId,
                    match.Title,
                    match.Category,
                    match.LinkToPhoto,
                    DetectedItemDescription = detectedItem.description?.ToString() ?? ""
                });
            }
        }

        return matchingItems;
    }

    private string BuildStylingTipsPrompt(ClothingItem item, List<ClothingItem> otherItems)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"You are a fashion stylist. Provide styling tips for this item:");
        sb.AppendLine($"Item: {item.Title} ({item.Category})");
        sb.AppendLine($"Tags: {string.Join(", ", item.Tags ?? new List<string>())}");
        
        if (item.AIAnalysis != null)
        {
            sb.AppendLine($"Colors: {string.Join(", ", item.AIAnalysis.DetectedColors ?? new List<string>())}");
            sb.AppendLine($"Patterns: {string.Join(", ", item.AIAnalysis.DetectedPatterns ?? new List<string>())}");
        }

        sb.AppendLine("\nOther items in closet:");
        foreach (var other in otherItems.Take(20))
        {
            sb.AppendLine($"- ID: {other.ClothingItemId}, Title: {other.Title}, Category: {other.Category}");
        }

        sb.AppendLine("\nProvide: general_styling_tips (array), pairing_suggestions (array with item_id and reason), occasions (array)");
        sb.AppendLine("Return as JSON.");

        return sb.ToString();
    }

    private async Task<object> GetStylingTipsFromGPTAsync(string prompt)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a professional fashion stylist AI."),
                new UserChatMessage(prompt)
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 1000
            });

            var content = chatCompletion.Value.Content[0].Text;

            return JsonConvert.DeserializeObject<dynamic>(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting styling tips from GPT");
            return new { };
        }
    }

    private string BuildWardrobeGapPrompt(List<ClothingItem> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a fashion consultant. Analyze this wardrobe and identify missing essentials:");
        
        var categories = items.GroupBy(x => x.Category);
        foreach (var cat in categories)
        {
            sb.AppendLine($"- {cat.Key}: {cat.Count()} items");
        }

        sb.AppendLine("\nProvide: missing_essentials (array with category, item_type, reasoning, priority: high/medium/low)");
        sb.AppendLine("Return as JSON.");

        return sb.ToString();
    }

    private async Task<object> IdentifyGapsWithGPTAsync(string prompt)
    {
        try
        {
            if (_aiClient == null)
            {
                throw new InvalidOperationException("AI client is not configured. Please configure Azure OpenAI settings.");
            }

            var chatClient = _aiClient.GetChatClient(_modelDeploymentName);
            
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a fashion consultant AI."),
                new UserChatMessage(prompt)
            };

            var chatCompletion = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                Temperature = 0.7f,
                MaxOutputTokenCount = 1000
            });

            var content = chatCompletion.Value.Content[0].Text;

            return JsonConvert.DeserializeObject<dynamic>(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying gaps with GPT");
            return new { };
        }
    }

    #endregion
}
