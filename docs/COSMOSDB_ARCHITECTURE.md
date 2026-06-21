# MyCloset CosmosDB Architecture

## Overview

This document describes the CosmosDB NoSQL data model, partition strategy, and best practices for the MyCloset application.

## Why CosmosDB?

- **Global Distribution**: Multi-region replication for low-latency worldwide access
- **Elastic Scalability**: Auto-scale throughput based on demand
- **NoSQL Flexibility**: Schema-less document model for rapid iteration
- **High Performance**: Single-digit millisecond read/write latency
- **Cost-Effective**: Request Unit (RU) pricing model

## Container Architecture

### Container Strategy

We use **6 containers** with partition keys optimized for query patterns:

| Container | Partition Key | Description | Avg Doc Size |
|-----------|---------------|-------------|--------------|
| **Users** | `/id` | User profiles | ~1 KB |
| **ClothingItems** | `/userId` | Clothing inventory | ~2-5 KB |
| **Outfits** | `/userId` | Outfit compositions | ~10-20 KB |
| **OutfitRecommendations** | `/userId` | AI recommendations | ~5-10 KB |
| **SocialMediaData** | `/userId` | Connections + Posts (shared) | ~2-5 KB |
| **Friendships** | `/user1` | Friend relationships | ~2 KB |

### Partition Key Design Principles

#### ✅ **High Cardinality**
- **userId** as partition key ensures one partition per user
- Scales to millions of users without hotspots
- Each user's data is co-located for fast queries

#### ✅ **Query Pattern Alignment**
- 90% of queries filter by userId (logged-in user context)
- No cross-partition queries for primary use cases
- Fast single-partition reads (5-10ms)

#### ✅ **Size Limits**
- Each user's data well under 20GB partition limit
- Average user: ~1,000 items × 5KB = 5MB total

## Document Models

### 1. User (Container: Users, PK: /id)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "User",
  "displayName": "Jane Doe",
  "email": "jane@example.com",
  "accountProvider": "Google",
  "isPublic": true,
  "profilePhotoUrl": "https://...",
  "dateAdded": "2026-01-15T10:30:00Z",
  "lastModified": "2026-06-20T14:22:00Z",
  "lastLogin": "2026-06-21T09:15:00Z",
  "_etag": "\"00000000-0000-0000-0000-000000000000\""
}
```

**Design Notes:**
- `id` = `userId` for simple lookups
- `type` discriminator for future polymorphism
- `_etag` for optimistic concurrency

### 2. ClothingItem (Container: ClothingItems, PK: /userId)

```json
{
  "id": "item-123e4567-e89b-12d3-a456-426614174000",
  "clothingItemId": "123e4567-e89b-12d3-a456-426614174000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "ClothingItem",
  "title": "Blue Denim Jacket",
  "description": "Vintage oversized fit",
  "category": "Outerwear",
  "color": "Blue",
  "brand": "Levi's",
  "size": "M",
  "season": "Fall/Spring",
  "linkToPhoto": "https://...",
  "tags": ["casual", "vintage", "denim"],
  "dateAdded": "2026-02-10T12:00:00Z",
  "lastModified": "2026-03-15T08:30:00Z",
  "aiAnalysis": {
    "analyzedAt": "2026-02-10T12:05:00Z",
    "detectedColors": ["#4169E1", "#1E3A5F"],
    "detectedPatterns": ["solid"],
    "suggestedTags": ["casual", "denim", "jacket"],
    "stylingTips": "Pair with white t-shirt and black jeans",
    "confidence": 0.95
  },
  "_etag": "\"...\""
}
```

**Design Notes:**
- **Embedded AI Analysis**: Always queried together, no separate lookup needed
- **Tags as Array**: Native JSON array for efficient queries
- **Partition by userId**: All user's items in same partition

### 3. Outfit (Container: Outfits, PK: /userId)

```json
{
  "id": "outfit-789e4567-e89b-12d3-a456-426614174000",
  "outfitId": "789e4567-e89b-12d3-a456-426614174000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "Outfit",
  "name": "Casual Friday",
  "occasion": "Work Casual",
  "season": "Spring",
  "notes": "Perfect for warm spring days",
  "tags": ["work", "casual", "spring"],
  "dateCreated": "2026-03-20T10:00:00Z",
  "lastModified": "2026-04-05T15:30:00Z",
  "clothingItems": [
    {
      "clothingItemId": "123e4567-e89b-12d3-a456-426614174000",
      "title": "Blue Denim Jacket",
      "category": "Outerwear",
      "linkToPhoto": "https://..."
    },
    {
      "clothingItemId": "456e7890-e89b-12d3-a456-426614174000",
      "title": "White Cotton T-Shirt",
      "category": "Tops",
      "linkToPhoto": "https://..."
    }
  ],
  "wornHistory": [
    {
      "dateWorn": "2026-03-22T09:00:00Z",
      "location": "Office",
      "occasion": "Work",
      "notes": "Got compliments!",
      "socialMediaPostId": null
    }
  ],
  "_etag": "\"...\""
}
```

**Design Notes:**
- **Embedded ClothingItemReference**: Denormalized for fast display
- **Embedded WornHistory**: Avoids separate container for history records
- **Trade-off**: Duplicates item info, but eliminates 3+ database queries

### 4. OutfitRecommendation (Container: OutfitRecommendations, PK: /userId)

```json
{
  "id": "rec-abc12345-e89b-12d3-a456-426614174000",
  "recommendationId": "abc12345-e89b-12d3-a456-426614174000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "OutfitRecommendation",
  "occasion": "Date Night",
  "weather": "Cool Evening",
  "season": "Fall",
  "reasoning": "These pieces create a sophisticated look...",
  "confidenceScore": 88,
  "isAccepted": false,
  "dateGenerated": "2026-06-21T10:30:00Z",
  "recommendedItems": [
    {
      "clothingItemId": "123e4567-...",
      "title": "Black Leather Jacket",
      "category": "Outerwear",
      "linkToPhoto": "https://...",
      "reasonForSelection": "Adds edge to the outfit"
    }
  ],
  "_etag": "\"...\""
}
```

### 5. SocialMediaData Container (PK: /userId)

**Shared container** with type discriminator for both connections and posts:

#### SocialMediaConnection (type: "SocialMediaConnection")

```json
{
  "id": "conn-def45678-e89b-12d3-a456-426614174000",
  "connectionId": "def45678-e89b-12d3-a456-426614174000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "SocialMediaConnection",
  "platform": "Instagram",
  "externalUserId": "ig_12345678",
  "username": "jane_closet",
  "profileUrl": "https://instagram.com/jane_closet",
  "accessToken": "<encrypted>",
  "refreshToken": "<encrypted>",
  "tokenExpiresAt": "2026-07-21T10:30:00Z",
  "scopes": "user_profile,user_media",
  "isActive": true,
  "dateConnected": "2026-01-15T12:00:00Z",
  "lastSynced": "2026-06-21T09:00:00Z",
  "_etag": "\"...\""
}
```

#### SocialMediaPost (type: "SocialMediaPost")

```json
{
  "id": "post-ghi78901-e89b-12d3-a456-426614174000",
  "socialMediaPostId": "ghi78901-e89b-12d3-a456-426614174000",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "type": "SocialMediaPost",
  "outfitId": "789e4567-e89b-12d3-a456-426614174000",
  "platform": "Instagram",
  "postUrl": "https://instagram.com/p/ABC123",
  "postDate": "2026-03-22T18:00:00Z",
  "caption": "Loving this casual Friday look! #OOTD",
  "likesCount": 245,
  "commentsCount": 12,
  "imageUrl": "https://...",
  "dateAdded": "2026-03-22T18:05:00Z",
  "lastSynced": "2026-06-21T09:00:00Z",
  "_etag": "\"...\""
}
```

**Design Notes:**
- **Type Discriminator**: `"type": "SocialMediaConnection"` vs `"SocialMediaPost"`
- **Shared Container**: Reduces container count and RU costs
- **Same Partition Key**: Both scoped to userId

### 6. Friendship (Container: Friendships, PK: /user1)

```json
{
  "id": "friend-jkl01234-e89b-12d3-a456-426614174000",
  "friendshipId": "jkl01234-e89b-12d3-a456-426614174000",
  "user1": "550e8400-e29b-41d4-a716-446655440000",
  "user2": "660f9511-f39c-52e5-b827-557766551111",
  "type": "Friendship",
  "requestStatus": 2,
  "createdOn": "2026-02-01T10:00:00Z",
  "createdBy": "550e8400-e29b-41d4-a716-446655440000",
  "modifiedOn": "2026-02-02T14:30:00Z",
  "modifiedBy": "660f9511-f39c-52e5-b827-557766551111",
  "user1Info": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "displayName": "Jane Doe",
    "email": "jane@example.com",
    "profilePhotoUrl": "https://..."
  },
  "user2Info": {
    "userId": "660f9511-f39c-52e5-b827-557766551111",
    "displayName": "John Smith",
    "email": "john@example.com",
    "profilePhotoUrl": "https://..."
  },
  "_etag": "\"...\""
}
```

**Design Notes:**
- **Embedded User Info**: Denormalized for fast friend list display
- **Bidirectional Lookup**: Create duplicate doc with swapped user1/user2 for both users' queries
- **Status Enum**: 1=Pending, 2=Accepted, 3=Rejected, 4=Blocked

## Query Patterns & Performance

### Primary Queries (Single-Partition)

| Query | Container | Partition Key Filter | Performance |
|-------|-----------|---------------------|-------------|
| Get user's clothing | ClothingItems | userId = X | 5-10ms |
| Get user's outfits | Outfits | userId = X | 5-10ms |
| Get AI recommendations | OutfitRecommendations | userId = X | 5-10ms |
| Get social connections | SocialMediaData | userId = X, type = "SocialMediaConnection" | 5-10ms |
| Get user's posts | SocialMediaData | userId = X, type = "SocialMediaPost" | 5-10ms |

### Secondary Queries (Cross-Partition)

⚠️ **Avoid these in hot paths** (use cache or denormalization):

| Query | Issue | Solution |
|-------|-------|----------|
| Search all users by email | Cross-partition query | Add secondary index or Azure Search |
| Find items by tag globally | Scans all partitions | Denormalize popular tags into separate container |
| Get all posts for an outfit | OutfitId not in partition key | Embed post references in Outfit document |

## Data Denormalization Strategy

### When to Denormalize

✅ **Embed** when data is:
- Always read together (ClothingItem → AIAnalysis)
- Small in size (<100 KB)
- Rarely updated independently
- 1:Few relationship (Outfit → WornHistory)

❌ **Reference** when data is:
- Large (>100 KB)
- Frequently updated independently
- Many-to-many relationships
- Shared across entities

### Examples

| Original SQL Pattern | CosmosDB Solution | Reason |
|---------------------|-------------------|---------|
| AIImageAnalysis table | Embedded in ClothingItem | Always queried together |
| OutfitHistory table | Embedded array in Outfit | Small, rarely updated |
| OutfitClothingItems join table | Embedded references in Outfit | Fast display, denormalized item details |
| User table | Separate container | Large entity, frequently updated |

## Configuration

### appsettings.json

```json
{
  "CosmosDb": {
    "ConnectionString": "AccountEndpoint=https://myclosetapp.documents.azure.com:443/;AccountKey=...",
    "DatabaseName": "MyClosetDB"
  }
}
```

### Program.cs

```csharp
builder.Services.AddDbContext<MyClosetAppDbContext>(options =>
{
    var cosmosConnectionString = configuration["CosmosDb:ConnectionString"];
    var databaseName = configuration["CosmosDb:DatabaseName"] ?? "MyClosetDB";

    options.UseCosmos(
        connectionString: cosmosConnectionString!,
        databaseName: databaseName
    );
});
```

## Request Unit (RU) Cost Estimates

### Read Operations

| Operation | RUs | Notes |
|-----------|-----|-------|
| Get user profile by id | 1 RU | Point read (most efficient) |
| Get user's 50 clothing items | 5-10 RUs | Single-partition query |
| Get user's 20 outfits | 10-15 RUs | Includes embedded items |
| Get AI recommendation | 2-5 RUs | Point read |

### Write Operations

| Operation | RUs | Notes |
|-----------|-----|-------|
| Create clothing item | 5-10 RUs | With embedded AI analysis |
| Update outfit | 10-15 RUs | With embedded worn history |
| Create user | 5 RUs | Small document |
| Batch insert 10 items | 50-100 RUs | Use bulk operations |

### Optimization Tips

1. **Use Point Reads**: Provide both `id` and partition key for 1 RU reads
2. **Batch Operations**: Use bulk insert for multiple documents
3. **Index Only What You Query**: Disable indexing on large text fields
4. **Cache Frequently Read Data**: Use Redis for user profiles
5. **Prefer Single-Partition Queries**: Design for userId-scoped queries

## Indexing Strategy

### Default Indexing Policy

CosmosDB indexes all properties by default. For large text fields, consider excluding:

```csharp
modelBuilder.Entity<ClothingItem>()
    .ToContainer("ClothingItems")
    .HasPartitionKey(c => c.UserId)
    .Property(c => c.Description)
    .HasJsonPropertyName("description")
    .IsRequired(false);
```

### Composite Indexes

For common query patterns like "Get user's items by category":

```json
{
  "compositeIndexes": [
    [
      { "path": "/userId", "order": "ascending" },
      { "path": "/category", "order": "ascending" }
    ]
  ]
}
```

## Migration from SQL Server

### Key Changes

1. **No Foreign Keys**: Use embedded references or ID-based lookups in service layer
2. **No Joins**: Denormalize related data into documents
3. **Partition Key Required**: Every query should filter by partition key
4. **id Property**: CosmosDB requires lowercase `id` property
5. **ETag for Concurrency**: Use `_etag` instead of row versioning

### Migration Steps

1. ✅ Add `Microsoft.EntityFrameworkCore.Cosmos` package
2. ✅ Convert models with `[JsonProperty("id")]` and partition keys
3. ✅ Update DbContext to use `.ToContainer()` and `.HasPartitionKey()`
4. ✅ Embed related data (AIAnalysis, WornHistory)
5. ✅ Update Program.cs to use `UseCosmos()`
6. ⚠️ Update service layer to handle denormalized data
7. ⚠️ Test query patterns for RU consumption
8. ⚠️ Set up Azure CosmosDB account and provision containers

## Best Practices

### ✅ Do

- Partition by `userId` for user-scoped data
- Embed small related entities (<100 KB)
- Use `id` + partition key for point reads (1 RU)
- Use ETags for optimistic concurrency
- Cache frequently read data (user profiles)
- Use bulk operations for batch inserts
- Monitor RU consumption in production

### ❌ Don't

- Use cross-partition queries in hot paths
- Store documents >2 MB (hard limit)
- Update denormalized data without cascading updates
- Ignore partition key in queries (triggers fan-out)
- Over-index large text fields
- Use joins (not supported)
- Rely on SQL-style transactions across containers

## Next Steps

1. **Provision CosmosDB Account**:
   ```bash
   az cosmosdb create --name myclosetapp --resource-group MyClosetRG
   ```

2. **Create Database and Containers**:
   ```bash
   az cosmosdb sql database create --account-name myclosetapp --name MyClosetDB
   az cosmosdb sql container create --account-name myclosetapp \
     --database-name MyClosetDB --name ClothingItems \
     --partition-key-path /userId --throughput 400
   ```

3. **Update appsettings.json** with connection string

4. **Migrate Existing Data** (if applicable):
   - Export SQL data to JSON
   - Transform to CosmosDB document format
   - Bulk import using Azure Data Factory or custom script

5. **Update Service Layer**:
   - Handle denormalized data updates
   - Implement cascade updates for embedded references
   - Add retry logic for transient failures

6. **Test & Monitor**:
   - Verify query performance
   - Monitor RU consumption
   - Set up alerts for throttling (429 errors)
   - Configure auto-scale for production load

## Resources

- [CosmosDB Best Practices](https://learn.microsoft.com/azure/cosmos-db/nosql/best-practice)
- [Partition Key Design](https://learn.microsoft.com/azure/cosmos-db/partitioning-overview)
- [EF Core Cosmos Provider](https://learn.microsoft.com/ef/core/providers/cosmos/)
- [Request Units](https://learn.microsoft.com/azure/cosmos-db/request-units)
