using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB OutfitRecommendation Document
/// Container: OutfitRecommendations
/// Partition Key: /userId
/// </summary>
public partial class OutfitRecommendation
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("recommendationId")]
    public Guid RecommendationId { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "OutfitRecommendation";

    /// <summary>
    /// Occasion or context for this outfit
    /// </summary>
    [JsonProperty("occasion")]
    public string Occasion { get; set; } = null!;

    /// <summary>
    /// Weather conditions this outfit is suitable for
    /// </summary>
    [JsonProperty("weather")]
    public string? Weather { get; set; }

    /// <summary>
    /// Season this outfit is best for
    /// </summary>
    [JsonProperty("season")]
    public string? Season { get; set; }

    /// <summary>
    /// AI reasoning for this recommendation
    /// </summary>
    [JsonProperty("reasoning")]
    public string Reasoning { get; set; } = null!;

    /// <summary>
    /// AI confidence score (0-100)
    /// </summary>
    [JsonProperty("confidenceScore")]
    public int ConfidenceScore { get; set; }

    /// <summary>
    /// Whether user accepted and saved this recommendation
    /// </summary>
    [JsonProperty("isAccepted")]
    public bool IsAccepted { get; set; }

    [JsonProperty("dateGenerated")]
    public DateTime DateGenerated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Embedded clothing item references with AI reasoning
    /// </summary>
    [JsonProperty("recommendedItems")]
    public List<RecommendedClothingItem> RecommendedItems { get; set; } = new();

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}

/// <summary>
/// Recommended clothing item with AI reasoning (embedded)
/// </summary>
public class RecommendedClothingItem
{
    [JsonProperty("clothingItemId")]
    public Guid ClothingItemId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("category")]
    public string Category { get; set; } = null!;

    [JsonProperty("linkToPhoto")]
    public string LinkToPhoto { get; set; } = null!;

    [JsonProperty("reasonForSelection")]
    public string? ReasonForSelection { get; set; }
}
