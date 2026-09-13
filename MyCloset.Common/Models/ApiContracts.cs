namespace MyCloset.Common.Models;

public sealed class ImageAnalysisRequest
{
    public Guid ClothingItemId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public sealed class OutfitRecommendationRequest
{
    public string? Occasion { get; set; }
    public string? Weather { get; set; }
    public string? Season { get; set; }
    public List<Guid>? ExcludeItemIds { get; set; }
}

public sealed class LinkSocialMediaRequest
{
    public string Platform { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}

public sealed class RecordOutfitWornRequest
{
    public Guid OutfitId { get; set; }
    public DateTime DateWorn { get; set; }
    public string? Location { get; set; }
    public string? Occasion { get; set; }
    public string? Notes { get; set; }
    public Guid? SocialMediaPostId { get; set; }
}

public sealed class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Provider { get; set; } = "Google";
}

public sealed class LoginResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
