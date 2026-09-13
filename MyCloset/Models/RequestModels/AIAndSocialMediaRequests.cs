using MyCloset.Common.Models;

namespace MyCloset.Models.RequestModels;

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
/// Request for adding a social media post
/// </summary>
public class AddSocialMediaPostRequest
{
    public string PostUrl { get; set; } = null!;
    public string Platform { get; set; } = null!;
    public Guid? OutfitId { get; set; }
}

/// <summary>
/// Request for finding similar outfits
/// </summary>
public class FindSimilarOutfitsRequest
{
    public string ImageUrl { get; set; } = null!;
}
