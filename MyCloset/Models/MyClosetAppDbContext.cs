using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyCloset.Models.DBModels;

public partial class MyClosetAppDbContext : DbContext
{
    public MyClosetAppDbContext()
    {
    }

    public MyClosetAppDbContext(DbContextOptions<MyClosetAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClothingItem> ClothingItems { get; set; }

    public virtual DbSet<FriendRequestStatus> FriendRequestStatuses { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Outfit> Outfits { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClothingItem>(entity =>
        {
            entity.HasKey(e => e.ClothingItemId).HasName("PK_ClothingItems_ClothingItemID");

            entity.Property(e => e.ClothingItemId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ClothingItemID");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LinkToPhoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tags).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ClothingItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClothingItems_UserID");
        });

        modelBuilder.Entity<FriendRequestStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("PK_FriendRequestStatus_StatusID");

            entity.ToTable("FriendRequestStatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedNever()
                .HasColumnName("StatusID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("PK_Friendship_FriendshipID");

            entity.ToTable("Friendship");

            entity.Property(e => e.FriendshipId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("FriendshipID");

            entity.HasOne(d => d.RequestStatusNavigation).WithMany(p => p.Friendships)
                .HasForeignKey(d => d.RequestStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_RequestStatus");

            entity.HasOne(d => d.RequestedNavigation).WithMany(p => p.FriendshipRequestedNavigations)
                .HasForeignKey(d => d.User1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_Requested");

            entity.HasOne(d => d.RequestorNavigation).WithMany(p => p.FriendshipRequestorNavigations)
                .HasForeignKey(d => d.User2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friendship_Requestor");
        });

        modelBuilder.Entity<Outfit>(entity =>
        {
            entity.HasKey(e => e.OutfitId).HasName("PK__Outfits__399B99D1ED054789");

            entity.Property(e => e.OutfitId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("OutfitID");
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Tags).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Outfits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Outfits__UserID__367C1819");

            entity.HasMany(d => d.ClothingItems).WithMany(p => p.Outfits)
                .UsingEntity<Dictionary<string, object>>(
                    "OutfitClothingItem",
                    r => r.HasOne<ClothingItem>().WithMany()
                        .HasForeignKey("ClothingItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OutfitClothingItems_ClothingItemID"),
                    l => l.HasOne<Outfit>().WithMany()
                        .HasForeignKey("OutfitId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_OutfitClothingItems_OutfitID"),
                    j =>
                    {
                        j.HasKey("OutfitId", "ClothingItemId").HasName("PK__OutfitCl__D0A643FCC72F55D1");
                        j.ToTable("OutfitClothingItems");
                        j.IndexerProperty<Guid>("OutfitId").HasColumnName("OutfitID");
                        j.IndexerProperty<Guid>("ClothingItemId").HasColumnName("ClothingItemID");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_Users_UserID");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UserID");
            entity.Property(e => e.AccountProvider)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastLogin)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
