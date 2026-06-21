using System;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB SocialMediaPost Document
/// Container: SocialMediaData
/// Partition Key: /userId
/// Uses type discriminator to share container with SocialMediaConnection
/// </summary>
public partial class SocialMediaPost
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("socialMediaPostId")]
    public Guid SocialMediaPostId { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator - "SocialMediaPost"
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "SocialMediaPost";

    [JsonProperty("outfitId")]
    public Guid? OutfitId { get; set; }

    [JsonProperty("platform")]
    public string Platform { get; set; } = null!;

    [JsonProperty("postUrl")]
    public string PostUrl { get; set; } = null!;

    [JsonProperty("postDate")]
    public DateTime PostDate { get; set; }

    [JsonProperty("caption")]
    public string? Caption { get; set; }

    [JsonProperty("likesCount")]
    public int? LikesCount { get; set; }

    [JsonProperty("commentsCount")]
    public int? CommentsCount { get; set; }

    [JsonProperty("imageUrl")]
    public string? ImageUrl { get; set; }

    [JsonProperty("dateAdded")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastSynced")]
    public DateTime LastSynced { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}
