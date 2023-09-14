using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<ClothingItem> ClothingItems { get; set; }

    public virtual DbSet<Outfit> Outfits { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFriend> UserFriends { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: Remove??
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClothingItem>(entity =>
        {
            entity.HasKey(e => e.ClothingItemId).HasName("PK__Clothing__93DDA2DF1EB8A378");

            entity.Property(e => e.ClothingItemId)
                .ValueGeneratedNever()
                .HasColumnName("ClothingItemID");
            entity.Property(e => e.Category)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.LinkToPhoto)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Occasions).IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.ClothingItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ClothingI__UserI__628FA481");
        });

        modelBuilder.Entity<Outfit>(entity =>
        {
            entity.HasKey(e => e.OutfitId).HasName("PK__Outfits__399B99D1BD092C11");

            entity.Property(e => e.OutfitId)
                .ValueGeneratedNever()
                .HasColumnName("OutfitID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Outfits)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Outfits__UserID__656C112C");

            entity.HasMany(d => d.ClothingItems).WithMany(p => p.Outfits)
                .UsingEntity<Dictionary<string, object>>(
                    "OutfitClothingItem",
                    r => r.HasOne<ClothingItem>().WithMany()
                        .HasForeignKey("ClothingItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OutfitClo__Cloth__693CA210"),
                    l => l.HasOne<Outfit>().WithMany()
                        .HasForeignKey("OutfitId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OutfitClo__Outfi__68487DD7"),
                    j =>
                    {
                        j.HasKey("OutfitId", "ClothingItemId").HasName("PK__OutfitCl__D0A643FC04BEE13A");
                        j.ToTable("OutfitClothingItems");
                        j.IndexerProperty<int>("OutfitId").HasColumnName("OutfitID");
                        j.IndexerProperty<int>("ClothingItemId").HasColumnName("ClothingItemID");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC9B2E3C2D");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.AccountProvider)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DisplayName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserFriend>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("PK__UserFrie__4D531A741FDCDBD9");

            entity.Property(e => e.FriendshipId)
                .ValueGeneratedNever()
                .HasColumnName("FriendshipID");
            entity.Property(e => e.UserId1).HasColumnName("UserID1");
            entity.Property(e => e.UserId2).HasColumnName("UserID2");

            entity.HasOne(d => d.UserId1Navigation).WithMany(p => p.UserFriendUserId1Navigations)
                .HasForeignKey(d => d.UserId1)
                .HasConstraintName("FK__UserFrien__UserI__5EBF139D");

            entity.HasOne(d => d.UserId2Navigation).WithMany(p => p.UserFriendUserId2Navigations)
                .HasForeignKey(d => d.UserId2)
                .HasConstraintName("FK__UserFrien__UserI__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
