using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB ClothingItem Document
/// Container: ClothingItems
/// Partition Key: /userId
/// </summary>
public partial class ClothingItem
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("clothingItemId")]
    public Guid ClothingItemId { get; set; }

    /// <summary>
    /// Partition key - ensures all user's items in same partition
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "ClothingItem";

    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    [JsonProperty("color")]
    public string? Color { get; set; }

    [JsonProperty("brand")]
    public string? Brand { get; set; }

    [JsonProperty("size")]
    public string? Size { get; set; }

    [JsonProperty("season")]
    public string? Season { get; set; }

    [JsonProperty("linkToPhoto")]
    public string LinkToPhoto { get; set; } = null!;

    [JsonProperty("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonProperty("dateAdded")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Embedded AI analysis result (always queried together)
    /// </summary>
    [JsonProperty("aiAnalysis")]
    public EmbeddedAIAnalysis? AIAnalysis { get; set; }

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}

/// <summary>
/// Embedded AI analysis data (denormalized for performance)
/// </summary>
public class EmbeddedAIAnalysis
{
    [JsonProperty("analyzedAt")]
    public DateTime AnalyzedAt { get; set; }

    [JsonProperty("detectedColors")]
    public List<string> DetectedColors { get; set; } = new();

    [JsonProperty("detectedPatterns")]
    public List<string> DetectedPatterns { get; set; } = new();

    [JsonProperty("suggestedTags")]
    public List<string> SuggestedTags { get; set; } = new();

    [JsonProperty("stylingTips")]
    public string? StylingTips { get; set; }

    [JsonProperty("confidence")]
    public double Confidence { get; set; }
}
