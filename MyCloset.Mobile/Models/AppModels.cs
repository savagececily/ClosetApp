using System;

namespace MyCloset.Mobile.Models;

public class ClothingItem
{
    public Guid ClothingItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string? Size { get; set; }
    public string? Season { get; set; }
    public string LinkToPhoto { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public DateTime DateAdded { get; set; }
}

public class Outfit
{
    public Guid OutfitId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Occasion { get; set; }
    public string? Season { get; set; }
    public string? Notes { get; set; }
    public DateTime DateCreated { get; set; }
    public List<ClothingItem> ClothingItems { get; set; } = new();
}

public class OutfitHistory
{
    public Guid OutfitHistoryId { get; set; }
    public Guid OutfitId { get; set; }
    public DateTime DateWorn { get; set; }
    public string? Location { get; set; }
    public string? Occasion { get; set; }
    public string? Notes { get; set; }
    public string? OutfitName { get; set; }
}

public class SocialMediaPost
{
    public Guid SocialMediaPostId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string PostUrl { get; set; } = string.Empty;
    public DateTime PostDate { get; set; }
    public string? Caption { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public string? ImageUrl { get; set; }
}

public class SocialMediaConnection
{
    public Guid ConnectionId { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? ProfileUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateConnected { get; set; }
    public DateTime LastSynced { get; set; }
    public bool TokenExpired { get; set; }
}

public class OutfitRecommendation
{
    public Guid RecommendationId { get; set; }
    public List<ClothingItem> Items { get; set; } = new();
    public string Reasoning { get; set; } = string.Empty;
    public int ConfidenceScore { get; set; }
    public string? Occasion { get; set; }
}

public class User
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
}
