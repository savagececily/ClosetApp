using System;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB SocialMediaConnection Document
/// Container: SocialMediaData
/// Partition Key: /userId
/// Uses type discriminator to share container with SocialMediaPost
/// </summary>
public partial class SocialMediaConnection
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("connectionId")]
    public Guid ConnectionId { get; set; }

    /// <summary>
    /// Partition key
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator - "SocialMediaConnection"
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "SocialMediaConnection";

    [JsonProperty("platform")]
    public string Platform { get; set; } = null!;

    [JsonProperty("externalUserId")]
    public string ExternalUserId { get; set; } = null!;

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("profileUrl")]
    public string? ProfileUrl { get; set; }

    /// <summary>
    /// Encrypted OAuth access token
    /// </summary>
    [JsonProperty("accessToken")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Encrypted OAuth refresh token
    /// </summary>
    [JsonProperty("refreshToken")]
    public string? RefreshToken { get; set; }

    [JsonProperty("tokenExpiresAt")]
    public DateTime? TokenExpiresAt { get; set; }

    [JsonProperty("scopes")]
    public string? Scopes { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonProperty("dateConnected")]
    public DateTime DateConnected { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastSynced")]
    public DateTime LastSynced { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}
