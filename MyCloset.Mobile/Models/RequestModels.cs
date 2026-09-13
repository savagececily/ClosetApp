using MyCloset.Common.Models;

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

// Response Models
