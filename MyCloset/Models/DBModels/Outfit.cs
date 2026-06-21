using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB Outfit Document
/// Container: Outfits
/// Partition Key: /userId
/// </summary>
public partial class Outfit
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("outfitId")]
    public Guid OutfitId { get; set; }

    /// <summary>
    /// Partition key - ensures all user's outfits in same partition
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "Outfit";

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("occasion")]
    public string? Occasion { get; set; }

    [JsonProperty("season")]
    public string? Season { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonProperty("dateCreated")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Embedded clothing item references (denormalized for fast retrieval)
    /// Store IDs and basic info to avoid cross-partition queries
    /// </summary>
    [JsonProperty("clothingItems")]
    public List<ClothingItemReference> ClothingItems { get; set; } = new();

    /// <summary>
    /// Embedded outfit history (times worn)
    /// </summary>
    [JsonProperty("wornHistory")]
    public List<OutfitWornRecord> WornHistory { get; set; } = new();

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}

/// <summary>
/// Reference to a clothing item (denormalized for performance)
/// </summary>
public class ClothingItemReference
{
    [JsonProperty("clothingItemId")]
    public Guid ClothingItemId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    [JsonProperty("linkToPhoto")]
    public string LinkToPhoto { get; set; } = null!;
}

/// <summary>
/// Record of when outfit was worn (embedded for fast queries)
/// </summary>
public class OutfitWornRecord
{
    [JsonProperty("dateWorn")]
    public DateTime DateWorn { get; set; }

    [JsonProperty("location")]
    public string? Location { get; set; }

    [JsonProperty("occasion")]
    public string? Occasion { get; set; }

    [JsonProperty("notes")]
    public string? Notes { get; set; }

    [JsonProperty("socialMediaPostId")]
    public Guid? SocialMediaPostId { get; set; }
}
