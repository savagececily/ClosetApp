using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB User Document
/// Container: Users
/// Partition Key: /id
/// </summary>
public partial class User
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    /// <summary>
    /// User identifier (same as id for easy querying)
    /// </summary>
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Type discriminator for polymorphic queries
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "User";

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = null!;

    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    [JsonProperty("accountProvider")]
    public string AccountProvider { get; set; } = null!;

    [JsonProperty("isPublic")]
    public bool IsPublic { get; set; } = true;

    [JsonProperty("dateAdded")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastModified")]
    public DateTime LastModified { get; set; } = DateTime.UtcNow;

    [JsonProperty("lastLogin")]
    public DateTime LastLogin { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Profile photo URL
    /// </summary>
    [JsonProperty("profilePhotoUrl")]
    public string? ProfilePhotoUrl { get; set; }

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}
