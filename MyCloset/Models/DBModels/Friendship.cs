using System;
using Newtonsoft.Json;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB Friendship Document
/// Container: Friendships
/// Partition Key: /user1 (requestor)
/// For bidirectional lookup, create duplicate document with swapped users
/// </summary>
public partial class Friendship
{
    /// <summary>
    /// CosmosDB document id (lowercase required)
    /// </summary>
    [JsonProperty("id")]
    public string id { get; set; } = null!;

    [JsonProperty("friendshipId")]
    public Guid FriendshipId { get; set; }

    /// <summary>
    /// Partition key - User that initiated the request
    /// </summary>
    [JsonProperty("user1")]
    public Guid User1 { get; set; }

    /// <summary>
    /// User that received the request
    /// </summary>
    [JsonProperty("user2")]
    public Guid User2 { get; set; }

    /// <summary>
    /// Type discriminator
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; } = "Friendship";

    /// <summary>
    /// Status: 1=Pending, 2=Accepted, 3=Rejected, 4=Blocked
    /// </summary>
    [JsonProperty("requestStatus")]
    public int RequestStatus { get; set; }

    [JsonProperty("createdOn")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [JsonProperty("createdBy")]
    public Guid CreatedBy { get; set; }

    [JsonProperty("modifiedOn")]
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

    [JsonProperty("modifiedBy")]
    public Guid ModifiedBy { get; set; }

    /// <summary>
    /// Embedded user info to avoid lookups (denormalized)
    /// </summary>
    [JsonProperty("user1Info")]
    public FriendInfo? User1Info { get; set; }

    [JsonProperty("user2Info")]
    public FriendInfo? User2Info { get; set; }

    /// <summary>
    /// ETag for optimistic concurrency
    /// </summary>
    [JsonProperty("_etag")]
    public string? ETag { get; set; }
}

/// <summary>
/// Embedded friend information (denormalized for performance)
/// </summary>
public class FriendInfo
{
    [JsonProperty("userId")]
    public Guid UserId { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = null!;

    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    [JsonProperty("profilePhotoUrl")]
    public string? ProfilePhotoUrl { get; set; }
}
