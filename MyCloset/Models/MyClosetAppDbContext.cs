using Microsoft.EntityFrameworkCore;
using MyCloset.Models.DBModels;

namespace MyCloset.Models;

public partial class MyClosetAppDbContext : DbContext
{
    public MyClosetAppDbContext()
    {
    }

    public MyClosetAppDbContext(DbContextOptions<MyClosetAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ClothingItem> ClothingItems { get; set; }
    public DbSet<Outfit> Outfits { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<FriendRequestStatus> FriendRequestStatuses { get; set; }
    public DbSet<OutfitClothingItem> OutfitClothingItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: Remove??
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships or add custom configurations here if needed.
        // For example, you can specify foreign keys and other constraints.
        modelBuilder.Entity<OutfitClothingItem>()
            .HasKey(oci => new { oci.OutfitID, oci.ClothingItemID });

        modelBuilder.Entity<OutfitClothingItem>()
            .HasOne(oci => oci.Outfit)
            .WithMany(o => o.OutfitClothingItems)
            .HasForeignKey(oci => oci.OutfitID);

        modelBuilder.Entity<OutfitClothingItem>()
            .HasOne(oci => oci.ClothingItem)
            .WithMany(ci => ci.OutfitClothingItems)
            .HasForeignKey(oci => oci.ClothingItemID);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.RequestorUser)
            .WithMany(u => u.SentFriendRequests)
            .HasForeignKey(f => f.Requestor);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.RequestedUser)
            .WithMany(u => u.ReceivedFriendRequests)
            .HasForeignKey(f => f.Requested);
    }
}
