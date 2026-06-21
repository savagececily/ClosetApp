using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyCloset.Models.DBModels;

/// <summary>
/// CosmosDB EF Core DbContext
/// Uses Microsoft.EntityFrameworkCore.Cosmos provider
/// Container configuration with partition keys
/// </summary>
public partial class MyClosetAppDbContext : DbContext
{
    public MyClosetAppDbContext()
    {
    }

    public MyClosetAppDbContext(DbContextOptions<MyClosetAppDbContext> options)
        : base(options)
    {
    }

    // Container: Users (Partition Key: /id)
    public virtual DbSet<User> Users { get; set; }

    // Container: ClothingItems (Partition Key: /userId)
    public virtual DbSet<ClothingItem> ClothingItems { get; set; }

    // Container: Outfits (Partition Key: /userId)
    public virtual DbSet<Outfit> Outfits { get; set; }

    // Container: OutfitRecommendations (Partition Key: /userId)
    public virtual DbSet<OutfitRecommendation> OutfitRecommendations { get; set; }

    // Container: SocialMediaData (Partition Key: /userId) - contains both connections and posts
    public virtual DbSet<SocialMediaConnection> SocialMediaConnections { get; set; }
    public virtual DbSet<SocialMediaPost> SocialMediaPosts { get; set; }

    // Container: Friendships (Partition Key: /user1)
    public virtual DbSet<Friendship> Friendships { get; set; }

    // Legacy/Obsolete - kept for backward compatibility
    [Obsolete("Use embedded data in Outfit.WornHistory instead")]
    public virtual DbSet<OutfitHistory>? OutfitHistories { get; set; }

    [Obsolete("Use embedded data in ClothingItem.AIAnalysis instead")]
    public virtual DbSet<AIImageAnalysis>? AIImageAnalyses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure CosmosDB containers with partition keys

        // Users Container - Partition by id
        modelBuilder.Entity<User>()
            .ToContainer("Users")
            .HasPartitionKey(u => u.id)
            .HasNoDiscriminator();

        // ClothingItems Container - Partition by userId
        modelBuilder.Entity<ClothingItem>()
            .ToContainer("ClothingItems")
            .HasPartitionKey(c => c.UserId)
            .HasNoDiscriminator();

        // Outfits Container - Partition by userId
        modelBuilder.Entity<Outfit>()
            .ToContainer("Outfits")
            .HasPartitionKey(o => o.UserId)
            .HasNoDiscriminator();

        // OutfitRecommendations Container - Partition by userId
        modelBuilder.Entity<OutfitRecommendation>()
            .ToContainer("OutfitRecommendations")
            .HasPartitionKey(r => r.UserId)
            .HasNoDiscriminator();

        // SocialMediaData Container - Shared container with type discriminator
        // Partition by userId
        modelBuilder.Entity<SocialMediaConnection>()
            .ToContainer("SocialMediaData")
            .HasPartitionKey(s => s.UserId)
            .HasDiscriminator<string>("type")
            .HasValue<SocialMediaConnection>("SocialMediaConnection");

        modelBuilder.Entity<SocialMediaPost>()
            .ToContainer("SocialMediaData")
            .HasPartitionKey(s => s.UserId)
            .HasDiscriminator<string>("type")
            .HasValue<SocialMediaPost>("SocialMediaPost");

        // Friendships Container - Partition by user1 (requestor)
        modelBuilder.Entity<Friendship>()
            .ToContainer("Friendships")
            .HasPartitionKey(f => f.User1)
            .HasNoDiscriminator();

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
