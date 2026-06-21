using System;

namespace MyCloset.Mobile.Models;

// Request Models
public class ClothingItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Size { get; set; }
    public string? Season { get; set; }
    public string LinkToPhoto { get; set; } = string.Empty;
    public string? Tags { get; set; }
}

public class OutfitRequest
{
    public string Name { get; set; } = string.Empty;
    public List<Guid> ClothingItemIds { get; set; } = new();
    public string? Occasion { get; set; }
    public string? Season { get; set; }
    public string? Notes { get; set; }
}

public class OutfitRecommendationRequest
{
    public string? Occasion { get; set; }
    public string? Weather { get; set; }
    public string? Season { get; set; }
    public List<Guid>? ExcludeItemIds { get; set; }
}

public class ImageAnalysisRequest
{
    public Guid ClothingItemId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class RecordOutfitWornRequest
{
    public Guid OutfitId { get; set; }
    public DateTime DateWorn { get; set; }
    public string? Location { get; set; }
    public string? Occasion { get; set; }
    public string? Notes { get; set; }
}

public class LinkSocialMediaRequest
{
    public string Platform { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
}

// Response Models
public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Provider { get; set; } = "Google"; // Google, Facebook, Microsoft
}

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
