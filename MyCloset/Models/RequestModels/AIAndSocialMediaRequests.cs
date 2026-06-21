namespace MyCloset.Models.RequestModels;

/// <summary>
/// Request for analyzing a clothing item image with AI
/// </summary>
public class ImageAnalysisRequest
{
    public Guid ClothingItemId { get; set; }
    public string ImageUrl { get; set; } = null!;
}

/// <summary>
/// Request for generating outfit recommendations
/// </summary>
public class OutfitRecommendationRequest
{
    public string Occasion { get; set; } = null!;
    public string? Weather { get; set; }
    public string? Season { get; set; }
    public List<Guid>? ExcludeItemIds { get; set; }
}

/// <summary>
/// Request for getting outfit completion suggestions
/// </summary>
public class OutfitCompletionRequest
{
    public List<Guid> SelectedItemIds { get; set; } = new List<Guid>();
    public string Occasion { get; set; } = null!;
}

/// <summary>
/// Request for analyzing a full outfit image
/// </summary>
public class OutfitImageAnalysisRequest
{
    public string ImageUrl { get; set; } = null!;
}

/// <summary>
/// Request for linking a social media account
/// </summary>
public class LinkSocialMediaRequest
{
    public string Platform { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}

/// <summary>
/// Request for adding a social media post
/// </summary>
public class AddSocialMediaPostRequest
{
    public string PostUrl { get; set; } = null!;
    public string Platform { get; set; } = null!;
    public Guid? OutfitId { get; set; }
}

/// <summary>
/// Request for recording when an outfit was worn
/// </summary>
public class RecordOutfitWornRequest
{
    public Guid OutfitId { get; set; }
    public DateTime DateWorn { get; set; }
    public string? Location { get; set; }
    public string? Occasion { get; set; }
    public string? Notes { get; set; }
    public Guid? SocialMediaPostId { get; set; }
}

/// <summary>
/// Request for finding similar outfits
/// </summary>
public class FindSimilarOutfitsRequest
{
    public string ImageUrl { get; set; } = null!;
}
